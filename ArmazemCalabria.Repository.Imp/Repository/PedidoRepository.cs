using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Repository.Imp.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository.Imp.Repository
{
    public class PedidoRepository(Context context) : RepositoryBase<Pedido>(context), IPedidoRepository
    {
        public async Task<List<Pedido>> ConsultarPedidos(int? idUsuarioSolicitante)
        {
            var query = Context.Pedidos
                .AsNoTracking()
                .Include(p => p.UsuarioSolicitante)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Piso)
                .AsQueryable();

            if (idUsuarioSolicitante.HasValue)
                query = query.Where(p => p.IdUsuarioSolicitante == idUsuarioSolicitante.Value);

            return await query
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }

        public async Task<Pedido?> ObterPedidoComItensParaAtualizacao(int idPedido)
        {
            return await Context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Piso)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
        }

        public async Task<Dictionary<int, (string Nome, int QuantidadeDisponivel)>> ObterDadosPisos(IEnumerable<int> idsPiso)
        {
            var ids = idsPiso.Distinct().ToList();

            return await Context.Pisos
                .AsNoTracking()
                .Where(p => ids.Contains(p.IdPiso))
                .ToDictionaryAsync(
                    p => p.IdPiso,
                    p => (p.Nome, p.QuantidadeDisponivel));
        }

        public async Task<bool> BaixarEstoque(int idPiso, int quantidade)
        {
            var linhasAfetadas = await Context.Database.ExecuteSqlRawAsync(
                "UPDATE armazem.tb_pisos SET quantidade_disponivel = quantidade_disponivel - {0} WHERE id_piso = {1} AND quantidade_disponivel >= {0}",
                quantidade,
                idPiso);

            return linhasAfetadas > 0;
        }
    }
}
