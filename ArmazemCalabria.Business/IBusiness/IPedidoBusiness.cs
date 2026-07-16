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

        /// <summary>
        /// Reavalia os pedidos pendentes após entrada de estoque (aprovação automática de sistema, sem contexto HTTP).
        /// Percorre os pendentes em ordem FIFO e aprova os que passaram a ter estoque suficiente.
        /// </summary>
        Task ReprocessarPedidosPendentes();
    }
}
