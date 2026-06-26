using ArmazemCalabria.Entity.Enum;

namespace ArmazemCalabria.Entity.Entities
{
    public class Pedido : IEntity
    {
        public int IdPedido { get; set; }
        public int IdUsuarioSolicitante { get; set; }
        public Usuario UsuarioSolicitante { get; set; }
        public StatusPedido IdStatus { get; set; }
        public string? MotivoRejeicao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int? IdUsuarioAprovador { get; set; }
        public Usuario? UsuarioAprovador { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataRejeicao { get; set; }

        public ICollection<PedidoItem> Itens { get; set; } = [];
    }
}
