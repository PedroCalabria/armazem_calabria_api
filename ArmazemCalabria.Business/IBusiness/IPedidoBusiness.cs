using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.Business.IBusiness
{
    public interface IPedidoBusiness
    {
        Task<List<PedidoGridItemDTO>> ConsultarPedidos();
        Task<PedidoCriadoDTO> SolicitarPedido(SolicitarPedidoDTO dto);
        Task AprovarPedido(int idPedido);
        Task<AprovarPedidosResultadoDTO> AprovarPedidos(AprovarPedidosDTO dto);
        Task RejeitarPedido(RejeitarPedidoDTO dto);
    }
}
