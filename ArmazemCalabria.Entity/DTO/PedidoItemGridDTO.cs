namespace ArmazemCalabria.Entity.DTO
{
    public class PedidoItemGridDTO
    {
        public int IdPedidoItem { get; set; }
        public int IdPiso { get; set; }
        public string NomePiso { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeDisponivel { get; set; }
    }
}
