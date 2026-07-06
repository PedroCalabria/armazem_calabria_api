using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities.Dominio;
using ArmazemCalabria.Entity.Entities.Importacao;
using ArmazemCalabria.Entity.Entities.Pisos;
using ArmazemCalabria.Repository.Imp.Repository.Base;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Utils.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository.Imp.Repository
{
    public class ArquivoImportacaoRepository(Context context)
        : RepositoryBase<ArquivoImportacao>(context), IArquivoImportacaoRepository
    {
        public async Task<int> SalvarAsync(ArquivoImportacao arquivo)
        {
            await InsertAsync(arquivo);
            return arquivo.IdArquivo;
        }

        public async Task<ArquivoImportacao?> ObterPorIdAsync(int idArquivo)
        {
            return await Entity.FirstOrDefaultAsync(a => a.IdArquivo == idArquivo);
        }

        public async Task AtualizarAsync(ArquivoImportacao arquivo)
        {
            await UpdateAsync(arquivo);
        }

        public async Task<List<Piso>> ObterPisosParaUpsertAsync(IEnumerable<string> nomes)
        {
            var listaNomes = nomes.Distinct().ToList();
            if (listaNomes.Count == 0)
                return [];

            // Collation padrão do SQL Server é case-insensitive; o refinamento da chave
            // (marca/cor/tipo) é feito na camada de negócio com texto normalizado.
            return await Context.Pisos
                .Where(p => listaNomes.Contains(p.Nome))
                .ToListAsync();
        }

        public async Task PersistirUpsertAsync(IEnumerable<Piso> novos)
        {
            // Pisos novos entram via AddRange (bulk insert). As quantidades dos pisos
            // existentes já foram alteradas nas entidades rastreadas por ObterPisosParaUpsertAsync,
            // então este mesmo SaveChanges persiste inserções e atualizações juntas.
            Context.AddRange(novos);
            await Context.SaveChangesAsync();
        }

        public async Task RegistrarErrosAsync(IEnumerable<ErroImportacao> erros)
        {
            Context.AddRange(erros);
            await Context.SaveChangesAsync();
        }

        public async Task<DominiosEstoqueDTO> ObterDominiosAsync()
        {
            var tiposPiso = await Context.Set<TipoPiso>().AsNoTracking()
                .Select(t => new { t.IdTipoPiso, t.Descricao }).ToListAsync();
            var marcas = await Context.Set<Marca>().AsNoTracking()
                .Select(m => new { m.IdMarca, m.Nome }).ToListAsync();
            var niveis = await Context.Set<NivelResistencia>().AsNoTracking()
                .Select(n => new { n.IdNivelResistencia, n.Descricao }).ToListAsync();
            var acabamentos = await Context.Set<Acabamento>().AsNoTracking()
                .Select(a => new { a.IdAcabamento, a.Descricao }).ToListAsync();
            var ambientes = await Context.Set<Ambiente>().AsNoTracking()
                .Select(a => new { a.IdAmbiente, a.Descricao }).ToListAsync();

            return new DominiosEstoqueDTO
            {
                TiposPiso = MontarDicionario(tiposPiso.Select(t => (t.Descricao, t.IdTipoPiso))),
                Marcas = MontarDicionario(marcas.Select(m => (m.Nome, m.IdMarca))),
                NiveisResistencia = MontarDicionario(niveis.Select(n => (n.Descricao, n.IdNivelResistencia))),
                Acabamentos = MontarDicionario(acabamentos.Select(a => (a.Descricao, a.IdAcabamento))),
                Ambientes = MontarDicionario(ambientes.Select(a => (a.Descricao, a.IdAmbiente)))
            };
        }

        private static Dictionary<string, int> MontarDicionario(IEnumerable<(string Descricao, int Id)> itens)
        {
            var dicionario = new Dictionary<string, int>();

            foreach (var (descricao, id) in itens)
            {
                var chave = TextoHelper.Normalizar(descricao);
                if (!string.IsNullOrEmpty(chave))
                    dicionario[chave] = id;
            }

            return dicionario;
        }
    }
}
