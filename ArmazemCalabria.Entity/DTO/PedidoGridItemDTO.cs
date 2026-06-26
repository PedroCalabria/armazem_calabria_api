namespace ArmazemCalabria.Entity.DTO
{
    public class PedidoGridItemDTO
    {
        public int IdPedido { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public string NomeSolicitante { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? MotivoRejeicao { get; set; }
        public bool PodeAprovar { get; set; }
        public bool PodeRejeitar { get; set; }
        public List<PedidoItemGridDTO> Itens { get; set; } = [];
    }
}
