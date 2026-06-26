using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Repository.IRepository.Base;

namespace ArmazemCalabria.Repository.IRepository
{
    public interface IPedidoRepository : IRepositoryBase<Pedido>
    {
        Task<List<Pedido>> ConsultarPedidos(int? idUsuarioSolicitante);
        Task<Pedido?> ObterPedidoComItensParaAtualizacao(int idPedido);
        Task<Dictionary<int, (string Nome, int QuantidadeDisponivel)>> ObterDadosPisos(IEnumerable<int> idsPiso);
        Task<bool> BaixarEstoque(int idPiso, int quantidade);
    }
}
