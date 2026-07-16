using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities.Importacao;
using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Repository.IRepository
{
    public interface IArquivoImportacaoRepository
    {
        Task<int> SalvarAsync(ArquivoImportacao arquivo);

        Task<ArquivoImportacao?> ObterPorIdAsync(int idArquivo);

        Task AtualizarAsync(ArquivoImportacao arquivo);

        /// <summary>
        /// Carrega (com tracking) os pisos existentes cujo nome esteja na lista informada,
        /// para resolução do upsert (atualizar quantidade de pisos que já existem).
        /// </summary>
        Task<List<Piso>> ObterPisosParaUpsertAsync(IEnumerable<string> nomes);

        /// <summary>
        /// Persiste o upsert: bulk insert dos pisos novos e, no mesmo SaveChanges,
        /// as alterações de quantidade dos pisos existentes (que já estão sendo rastreados).
        /// </summary>
        Task PersistirUpsertAsync(IEnumerable<Piso> novos);

        Task RegistrarErrosAsync(IEnumerable<ErroImportacao> erros);

        /// <summary>Carrega (sem tracking) os erros registrados para um arquivo, ordenados por linha.</summary>
        Task<List<ErroImportacao>> ObterErrosPorArquivoAsync(int idArquivo);

        /// <summary>Carrega os domínios (descrição normalizada -> id) para resolução dos textos da planilha.</summary>
        Task<DominiosEstoqueDTO> ObterDominiosAsync();
    }
}
