using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.Business.ICache;
using ArmazemCalabria.CrossCutting.Configurations;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Repository;
using ArmazemCalabria.Repository.IRepository;
using Confluent.Kafka;
using System.Text.Json;

namespace ArmazemCalabria.API.Messaging
{
    /// <summary>
    /// Consumer Kafka (Fase 5). Roda como BackgroundService dentro da própria API.
    /// Para cada evento de importação: processa o arquivo (leitura + bulk insert) e reprocessa
    /// os pedidos pendentes, tudo dentro de uma transação por mensagem.
    /// </summary>
    public class ImportacaoConsumerBackgroundService(
        IServiceScopeFactory _scopeFactory,
        KafkaSettings _settings,
        ILogger<ImportacaoConsumerBackgroundService> _logger) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consume() é bloqueante; roda em uma thread própria para não travar o startup do host.
            return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
        }

        private async Task ConsumeLoop(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false // commit manual do offset após processar com sucesso
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_settings.Topic);

            _logger.LogInformation(
                "Consumer de importação iniciado. Topic={Topic}, GroupId={GroupId}.",
                _settings.Topic, _settings.ConsumerGroupId);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? resultado;

                    try
                    {
                        resultado = consumer.Consume(stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Erro ao consumir mensagem do Kafka.");
                        continue;
                    }

                    if (resultado?.Message is null)
                        continue;

                    await ProcessarMensagem(resultado.Message.Value);

                    // Só confirma o offset após processar com sucesso (at-least-once).
                    consumer.Commit(resultado);
                }
            }
            catch (OperationCanceledException)
            {
                // Encerramento normal do host.
            }
            finally
            {
                consumer.Close();
                _logger.LogInformation("Consumer de importação encerrado.");
            }
        }

        private async Task ProcessarMensagem(string mensagem)
        {
            EventoImportacaoEstoqueDTO? evento;

            try
            {
                evento = JsonSerializer.Deserialize<EventoImportacaoEstoqueDTO>(mensagem);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Mensagem em formato inválido, ignorada. Conteúdo: {Mensagem}", mensagem);
                return;
            }

            if (evento is null || evento.IdArquivo <= 0)
            {
                _logger.LogWarning("Evento sem IdArquivo válido, ignorado. Conteúdo: {Mensagem}", mensagem);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionManager>();
            var importacaoBusiness = scope.ServiceProvider.GetRequiredService<IImportacaoEstoqueBusiness>();
            var estoqueCache = scope.ServiceProvider.GetRequiredService<IEstoqueCache>();

            try
            {
                await transactionManager.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

                var resultado = await importacaoBusiness.ProcessarImportacaoAsync(evento.IdArquivo);

                await transactionManager.CommitTransactionAsync();

                // Invalida o cache de estoque APÓS o commit (bulk insert + reprocessamento de pendentes).
                await estoqueCache.InvalidarAsync();

                _logger.LogInformation(
                    "Arquivo {IdArquivo} processado. Status={Status}, Inseridos={Inseridos}, Atualizados={Atualizados}, Erros={Erros}.",
                    evento.IdArquivo, resultado.StatusDescricao, resultado.TotalInseridos, resultado.TotalAtualizados, resultado.TotalErros);
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackTransactionsAsync();

                _logger.LogError(ex, "Falha ao processar o arquivo {IdArquivo}.", evento.IdArquivo);

                // O rollback desfez inclusive a atualização de status; marca a falha em um escopo novo.
                await MarcarFalha(evento.IdArquivo, ex.Message);
            }
        }

        private async Task MarcarFalha(int idArquivo, string mensagemErro)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IArquivoImportacaoRepository>();

                var arquivo = await repository.ObterPorIdAsync(idArquivo);
                if (arquivo is null)
                    return;

                arquivo.Status = StatusImportacao.Falha;
                arquivo.MensagemErro = mensagemErro.Length > 1000 ? mensagemErro[..1000] : mensagemErro;
                arquivo.DataProcessamento = DateTime.UtcNow;

                await repository.AtualizarAsync(arquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Não foi possível marcar o arquivo {IdArquivo} como Falha.", idArquivo);
            }
        }
    }
}
