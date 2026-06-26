using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities
{
    public class PedidoItem : IEntity
    {
        public int IdPedidoItem { get; set; }
        public int IdPedido { get; set; }
        public Pedido Pedido { get; set; }
        public int IdPiso { get; set; }
        public Piso Piso { get; set; }
        public int Quantidade { get; set; }
    }
}
