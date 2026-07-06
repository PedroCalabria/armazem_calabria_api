using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.CrossCutting;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities.Importacao;
using ArmazemCalabria.Entity.Entities.Pisos;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Entity.Helpers.Importacao;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Utils.Extensions;
using ArmazemCalabria.Utils.Helpers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace ArmazemCalabria.Business.Imp.Business
{
    public class ImportacaoEstoqueBusiness(
        IArquivoImportacaoRepository _repository,
        IUserContext _userContext,
        IHttpContextAccessor _httpContextAccessor) : IImportacaoEstoqueBusiness
    {
        private const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB
        private const string ExtensaoXlsx = ".xlsx";

        public async Task<ImportacaoEstoqueResultadoDTO> EnviarPlanilha(IFormFile arquivoEnviado)
        {
            if (arquivoEnviado is null || arquivoEnviado.Length == 0)
                throw new BusinessException("Arquivo não informado ou vazio.");

            using var memoryStream = new MemoryStream();
            await arquivoEnviado.CopyToAsync(memoryStream);

            var upload = new UploadPlanilhaDTO
            {
                NomeOriginal = arquivoEnviado.FileName,
                ContentType = arquivoEnviado.ContentType,
                TamanhoBytes = arquivoEnviado.Length,
                Conteudo = memoryStream.ToArray()
            };

            ValidarPermissao();
            ValidarArquivo(upload);

            var arquivo = new ArquivoImportacao
            {
                NomeOriginal = upload.NomeOriginal,
                ContentType = upload.ContentType,
                TamanhoBytes = upload.TamanhoBytes,
                Conteudo = upload.Conteudo,
                Status = StatusImportacao.Pendente,
                IdUsuarioEnvio = ObterIdUsuario(),
                DataEnvio = DateTime.UtcNow
            };

            var idArquivo = await _repository.SalvarAsync(arquivo);

            var resultado = await PublicarAsync(idArquivo);

            // Fluxo síncrono (Fase 4): o publicador devolve o resultado completo.
            // Fluxo assíncrono (Fase 5/Kafka): devolve null -> retornamos apenas o reconhecimento (Pendente).
            return resultado ?? new ImportacaoEstoqueResultadoDTO
            {
                IdArquivo = idArquivo,
                Status = StatusImportacao.Pendente.AsInt(),
                StatusDescricao = StatusImportacao.Pendente.GetDescription()
            };
        }

        #region validações

        private static void ValidarArquivo(UploadPlanilhaDTO upload)
        {
            if (upload?.Conteudo is null || upload.Conteudo.Length == 0)
                throw new BusinessException("Arquivo não informado ou vazio.");

            if (!upload.NomeOriginal.EndsWith(ExtensaoXlsx, StringComparison.OrdinalIgnoreCase))
                throw new BusinessException("Apenas arquivos .xlsx são aceitos.");

            if (upload.TamanhoBytes > TamanhoMaximoBytes)
                throw new BusinessException("O arquivo excede o tamanho máximo permitido (10 MB).");
        }

        private void ValidarPermissao()
        {
            var idPerfil = ObterIdPerfil();

            var permitido = idPerfil == TiposPerfilUsuario.Gestor.AsInt()
                || idPerfil == TiposPerfilUsuario.LogistaInterno.AsInt();

            if (!permitido)
                throw new BusinessException("Você não tem permissão para incluir estoque.");
        }

        #endregion

        #region contexto do usuário

        private int ObterIdUsuario()
        {
            var userId = _userContext.AdditionalData["UserId"]?.ToString();

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return id;
        }

        private int ObterIdPerfil()
        {
            var idPerfil = _userContext.AdditionalData["IdPerfil"]?.ToString();

            if (string.IsNullOrEmpty(idPerfil))
                idPerfil = _httpContextAccessor.HttpContext?.User.FindFirst("idRole")?.Value;

            if (string.IsNullOrEmpty(idPerfil) || !int.TryParse(idPerfil, out var id))
                throw new UnauthorizedAccessException("Perfil do usuário não identificado.");

            return id;
        }

        #endregion

        #region Leitor
        private ResultadoLeituraPlanilhaDTO Ler(byte[] conteudo)
        {
            var resultado = new ResultadoLeituraPlanilhaDTO();

            using var stream = new MemoryStream(conteudo);
            XLWorkbook workbook;
            try
            {
                workbook = new XLWorkbook(stream);
            }
            catch (Exception)
            {
                resultado.MensagemLayout = "Não foi possível ler o arquivo. Verifique se é um .xlsx válido.";
                return resultado;
            }

            using (workbook)
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();
                var headerRow = worksheet?.FirstRowUsed();

                if (worksheet is null || headerRow is null)
                {
                    resultado.MensagemLayout = "A planilha está vazia ou não possui linha de cabeçalho.";
                    return resultado;
                }

                // Mapa: descrição normalizada do cabeçalho -> número da coluna
                var mapaColunas = new Dictionary<string, int>();
                foreach (var celula in headerRow.CellsUsed())
                {
                    var chave = TextoHelper.Normalizar(celula.GetString());
                    if (!string.IsNullOrEmpty(chave) && !mapaColunas.ContainsKey(chave))
                        mapaColunas[chave] = celula.Address.ColumnNumber;
                }

                var faltantes = ColunasPlanilhaEstoqueHelper.TodasEsperadas
                    .Where(coluna => !mapaColunas.ContainsKey(TextoHelper.Normalizar(coluna)))
                    .ToList();

                if (faltantes.Count > 0)
                {
                    resultado.MensagemLayout =
                        $"Layout inválido. Colunas obrigatórias ausentes: {string.Join(", ", faltantes)}.";
                    return resultado;
                }

                resultado.LayoutValido = true;

                var primeiraLinhaDados = headerRow.RowNumber() + 1;
                var ultimaLinha = worksheet.LastRowUsed().RowNumber();

                for (var numeroLinha = primeiraLinhaDados; numeroLinha <= ultimaLinha; numeroLinha++)
                {
                    var row = worksheet.Row(numeroLinha);
                    if (row.IsEmpty())
                        continue;

                    var linha = new LinhaPlanilhaEstoqueDTO { NumeroLinha = numeroLinha };

                    foreach (var (chaveColuna, numeroColuna) in mapaColunas)
                        linha.Valores[chaveColuna] = row.Cell(numeroColuna).GetString().Trim();

                    resultado.Linhas.Add(linha);
                }
            }

            return resultado;
        }
        #endregion

        #region PublicadorImportacaoSincrono
        private async Task<ImportacaoEstoqueResultadoDTO?> PublicarAsync(int idArquivo)
        {
            return await ProcessarAsync(idArquivo);
        }
        #endregion

        #region ProcessadorImportacaoEstoque
        private async Task<ImportacaoEstoqueResultadoDTO> ProcessarAsync(int idArquivo)
        {
            var arquivo = await _repository.ObterPorIdAsync(idArquivo)
                ?? throw new BusinessException("Arquivo de importação não encontrado.");

            arquivo.Status = StatusImportacao.Processando;
            await _repository.AtualizarAsync(arquivo);

            var leitura = Ler(arquivo.Conteudo);

            if (!leitura.LayoutValido)
            {
                arquivo.Status = StatusImportacao.Falha;
                arquivo.MensagemErro = leitura.MensagemLayout;
                arquivo.DataProcessamento = DateTime.UtcNow;
                await _repository.AtualizarAsync(arquivo);

                return MontarResultado(arquivo, [], 0, 0);
            }

            var dominios = await _repository.ObterDominiosAsync();
            var pisosValidos = new List<Piso>();
            var erros = new List<ErroImportacao>();

            foreach (var linha in leitura.Linhas)
            {
                try
                {
                    pisosValidos.Add(ConstruirPiso(linha, dominios, arquivo.IdUsuarioEnvio));
                }
                catch (LinhaInvalidaException ex)
                {
                    erros.Add(new ErroImportacao
                    {
                        IdArquivo = arquivo.IdArquivo,
                        NumeroLinha = linha.NumeroLinha,
                        Coluna = ex.Coluna,
                        Mensagem = ex.Message,
                        ConteudoLinha = DescreverLinha(linha),
                        DataCriacao = DateTime.UtcNow
                    });
                }
            }

            var (novos, totalInseridos, totalAtualizados) =
                await ResolverUpsert(pisosValidos, arquivo.IdUsuarioEnvio);

            // Bulk insert dos novos + persistência das quantidades atualizadas (mesmo SaveChanges).
            await _repository.PersistirUpsertAsync(novos);

            if (erros.Count > 0)
                await _repository.RegistrarErrosAsync(erros);

            arquivo.TotalRegistros = leitura.Linhas.Count;
            arquivo.TotalSucesso = totalInseridos + totalAtualizados;
            arquivo.TotalErros = erros.Count;
            arquivo.Status = erros.Count == 0
                ? StatusImportacao.Processado
                : StatusImportacao.ProcessadoComErros;
            arquivo.DataProcessamento = DateTime.UtcNow;
            await _repository.AtualizarAsync(arquivo);

            return MontarResultado(arquivo, erros, totalInseridos, totalAtualizados);
        }

        #region upsert

        /// <summary>
        /// Resolve quais pisos são novos (inserção em lote) e quais já existem
        /// (somar a quantidade). Pisos repetidos dentro do próprio arquivo são agregados.
        /// </summary>
        private async Task<(List<Piso> Novos, int Inseridos, int Atualizados)> ResolverUpsert(
            List<Piso> pisosValidos, int idUsuario)
        {
            if (pisosValidos.Count == 0)
                return ([], 0, 0);

            var nomes = pisosValidos.Select(p => p.Nome).Distinct().ToList();
            var existentes = await _repository.ObterPisosParaUpsertAsync(nomes);

            var mapaExistentes = new Dictionary<string, Piso>();
            foreach (var piso in existentes)
                mapaExistentes.TryAdd(ChaveUpsert(piso), piso);

            var novos = new List<Piso>();
            var novosPorChave = new Dictionary<string, Piso>();
            var chavesAtualizadas = new HashSet<string>();

            foreach (var piso in pisosValidos)
            {
                var chave = ChaveUpsert(piso);

                if (mapaExistentes.TryGetValue(chave, out var existente))
                {
                    existente.QuantidadeDisponivel += piso.QuantidadeDisponivel;
                    existente.DataAlteracao = DateTime.UtcNow;
                    existente.IdUsuarioAlteracao = idUsuario;
                    chavesAtualizadas.Add(chave);
                }
                else if (novosPorChave.TryGetValue(chave, out var jaAdicionado))
                {
                    // mesma chave aparece mais de uma vez no arquivo: agrega quantidade
                    jaAdicionado.QuantidadeDisponivel += piso.QuantidadeDisponivel;
                }
                else
                {
                    novos.Add(piso);
                    novosPorChave[chave] = piso;
                }
            }

            return (novos, novos.Count, chavesAtualizadas.Count);
        }

        // Chave natural do piso: Nome + Marca + Cor + Tipo (texto normalizado/ids).
        private static string ChaveUpsert(Piso piso)
            => string.Join('|',
                TextoHelper.Normalizar(piso.Nome),
                piso.IdMarca,
                TextoHelper.Normalizar(piso.Cor),
                piso.IdTipoPiso);

        #endregion

        #region construção do piso

        private static Piso ConstruirPiso(LinhaPlanilhaEstoqueDTO linha, DominiosEstoqueDTO dominios, int idUsuario)
        {
            var idTipoPiso = ResolverDominio(linha, ColunasPlanilhaEstoqueHelper.TipoPiso, dominios.TiposPiso, "Tipo de piso");
            var tipo = (TiposPiso)idTipoPiso;

            Piso piso = tipo switch
            {
                TiposPiso.Ceramica => new PisoCeramica
                {
                    ClassePei = ParseIntObrigatorio(linha, ColunasPlanilhaEstoqueHelper.ClassePei)
                },
                TiposPiso.Porcelanato => new PisoPorcelanato
                {
                    FlagRetificado = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Retificado),
                    TipoPorcelanato = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.TipoPorcelanato)
                },
                TiposPiso.Vinilico => new PisoVinilico
                {
                    FlagAcustico = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Acustico),
                    TipoInstalacao = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.TipoInstalacao)
                },
                TiposPiso.Laminado => new PisoLaminado
                {
                    FlagResistenteCupim = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.ResistenteCupim)
                },
                TiposPiso.Madeira => new PisoMadeira
                {
                    TipoMadeira = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.TipoMadeira),
                    FlagMadeiraNobre = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.MadeiraNobre)
                },
                TiposPiso.PedraNatural => new PisoPedraNatural
                {
                    TipoPedra = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.TipoPedra),
                    FlagPorosidade = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Porosidade),
                    FlagNecessitaImpermeabilizacao =
                        ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.NecessitaImpermeabilizacao)
                },
                TiposPiso.CimentoQueimado => new PisoCimentoQueimado(),
                _ => throw new LinhaInvalidaException(
                    $"Tipo de piso não suportado.", ColunasPlanilhaEstoqueHelper.TipoPiso)
            };

            PreencherComuns(piso, linha, dominios, idUsuario, idTipoPiso);
            return piso;
        }

        private static void PreencherComuns(
            Piso piso, LinhaPlanilhaEstoqueDTO linha, DominiosEstoqueDTO dominios, int idUsuario, int idTipoPiso)
        {
            piso.Nome = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Nome);
            piso.Cor = ObterObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Cor);
            piso.Preco = ParseDecimalObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Preco);
            piso.Largura = ParseDecimalObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Largura);
            piso.Comprimento = ParseDecimalObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Comprimento);
            piso.Espessura = ParseDecimalObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Espessura);
            piso.Peso = ParseDecimalObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Peso);
            piso.FlagResistenteAgua = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.ResistenteAgua);
            piso.FlagAntiderrapante = ParseBoolObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Antiderrapante);
            piso.QuantidadeDisponivel = ParseIntObrigatorio(linha, ColunasPlanilhaEstoqueHelper.Quantidade);

            piso.IdTipoPiso = idTipoPiso;
            piso.IdMarca = ResolverDominio(linha, ColunasPlanilhaEstoqueHelper.Marca, dominios.Marcas, "Marca");
            piso.IdNivelResistencia =
                ResolverDominio(linha, ColunasPlanilhaEstoqueHelper.NivelResistencia, dominios.NiveisResistencia, "Nível de resistência");
            piso.IdAcabamento =
                ResolverDominio(linha, ColunasPlanilhaEstoqueHelper.Acabamento, dominios.Acabamentos, "Acabamento");
            piso.IdAmbiente =
                ResolverDominio(linha, ColunasPlanilhaEstoqueHelper.Ambiente, dominios.Ambientes, "Ambiente");

            piso.IdUsuarioCriacao = idUsuario;
            piso.DataCriacao = DateTime.UtcNow;
        }

        #endregion

        #region leitura/parse de células

        private static string? Valor(LinhaPlanilhaEstoqueDTO linha, string coluna)
        {
            return linha.Valores.TryGetValue(TextoHelper.Normalizar(coluna), out var valor)
                ? valor
                : null;
        }

        private static string ObterObrigatorio(LinhaPlanilhaEstoqueDTO linha, string coluna)
        {
            var valor = Valor(linha, coluna);
            if (string.IsNullOrWhiteSpace(valor))
                throw new LinhaInvalidaException($"'{coluna}' é obrigatório.", coluna);

            return valor.Trim();
        }

        private static decimal ParseDecimalObrigatorio(LinhaPlanilhaEstoqueDTO linha, string coluna)
        {
            var texto = ObterObrigatorio(linha, coluna);
            var normalizado = texto.Replace(",", ".");

            if (!decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                throw new LinhaInvalidaException($"'{coluna}' deve ser um número válido. Valor: '{texto}'.", coluna);

            if (valor < 0)
                throw new LinhaInvalidaException($"'{coluna}' não pode ser negativo.", coluna);

            return valor;
        }

        private static int ParseIntObrigatorio(LinhaPlanilhaEstoqueDTO linha, string coluna)
        {
            var texto = ObterObrigatorio(linha, coluna);

            if (!int.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                throw new LinhaInvalidaException($"'{coluna}' deve ser um número inteiro válido. Valor: '{texto}'.", coluna);

            if (valor < 0)
                throw new LinhaInvalidaException($"'{coluna}' não pode ser negativo.", coluna);

            return valor;
        }

        private static bool ParseBoolObrigatorio(LinhaPlanilhaEstoqueDTO linha, string coluna)
        {
            var texto = TextoHelper.Normalizar(ObterObrigatorio(linha, coluna));

            return texto switch
            {
                "true" or "verdadeiro" or "sim" or "s" or "1" or "x" => true,
                "false" or "falso" or "nao" or "n" or "0" => false,
                _ => throw new LinhaInvalidaException(
                    $"'{coluna}' deve ser Sim/Não (ou True/False). Valor: '{texto}'.", coluna)
            };
        }

        private static int ResolverDominio(
            LinhaPlanilhaEstoqueDTO linha, string coluna, Dictionary<string, int> dominio, string descricaoDominio)
        {
            var texto = ObterObrigatorio(linha, coluna);

            if (!dominio.TryGetValue(TextoHelper.Normalizar(texto), out var id))
                throw new LinhaInvalidaException($"{descricaoDominio} '{texto}' não está cadastrado(a).", coluna);

            return id;
        }

        #endregion

        #region resultado

        private static string DescreverLinha(LinhaPlanilhaEstoqueDTO linha)
        {
            var descricao = string.Join(" | ", linha.Valores.Values.Where(v => !string.IsNullOrWhiteSpace(v)));
            return descricao.Length > 2000 ? descricao[..2000] : descricao;
        }

        private static ImportacaoEstoqueResultadoDTO MontarResultado(
            ArquivoImportacao arquivo, List<ErroImportacao> erros, int totalInseridos, int totalAtualizados)
        {
            return new ImportacaoEstoqueResultadoDTO
            {
                IdArquivo = arquivo.IdArquivo,
                Status = arquivo.Status.AsInt(),
                StatusDescricao = arquivo.Status.GetDescription(),
                TotalRegistros = arquivo.TotalRegistros,
                TotalSucesso = arquivo.TotalSucesso,
                TotalInseridos = totalInseridos,
                TotalAtualizados = totalAtualizados,
                TotalErros = arquivo.TotalErros,
                MensagemErro = arquivo.MensagemErro,
                Erros = erros.Select(e => new ErroImportacaoDTO
                {
                    NumeroLinha = e.NumeroLinha,
                    Coluna = e.Coluna,
                    Mensagem = e.Mensagem
                }).ToList()
            };
        }

        #endregion

        #endregion
    }
}
