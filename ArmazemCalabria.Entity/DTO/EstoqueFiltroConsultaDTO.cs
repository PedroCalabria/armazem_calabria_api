namespace ArmazemCalabria.Entity.DTO
{
    public class EstoqueFiltroConsultaDTO
    {
        public List<int> IdsTipoPiso { get; set; } = [];
        public List<int> IdsMarca { get; set; } = [];
        public List<int> IdsNivelResistencia { get; set; } = [];
        public List<int> IdsAmbiente { get; set; } = [];
        public List<int> IdsAcabamento { get; set; } = [];
    }
}
