namespace ArmazemCalabria.Entity.DTO
{
    public class AprovarPedidosResultadoDTO
    {
        public List<int> Aprovados { get; set; } = [];
        public List<AprovarPedidoFalhaDTO> Falhas { get; set; } = [];
    }
}
