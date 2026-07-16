using ArmazemCalabria.Business.IMessaging;
using ArmazemCalabria.CrossCutting.Configurations;
using ArmazemCalabria.Entity.DTO;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ArmazemCalabria.Business.Imp.Messaging
{
    /// <summary>
    /// Publica o evento de importação de estoque no Kafka. Registrado como singleton:
    /// o <see cref="IProducer{TKey,TValue}"/> é thread-safe e de vida longa.
    /// </summary>
    public sealed class KafkaImportacaoEventPublisher : IImportacaoEventPublisher, IDisposable
    {
        private readonly KafkaSettings _settings;
        private readonly ILogger<KafkaImportacaoEventPublisher> _logger;
        private readonly IProducer<string, string> _producer;

        public KafkaImportacaoEventPublisher(
            KafkaSettings settings,
            ILogger<KafkaImportacaoEventPublisher> logger)
        {
            _settings = settings;
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublicarImportacaoAsync(int idArquivo)
        {
            var evento = new EventoImportacaoEstoqueDTO
            {
                IdArquivo = idArquivo,
                DataEnvio = DateTime.UtcNow
            };

            var mensagem = new Message<string, string>
            {
                Key = idArquivo.ToString(),
                Value = JsonSerializer.Serialize(evento)
            };

            var resultado = await _producer.ProduceAsync(_settings.Topic, mensagem);

            _logger.LogInformation(
                "Evento de importação publicado. IdArquivo={IdArquivo}, Topic={Topic}, Offset={Offset}.",
                idArquivo, _settings.Topic, resultado.Offset);
        }

        public void Dispose()
        {
            // Garante o flush das mensagens pendentes antes de encerrar.
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
