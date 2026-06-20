namespace ArmazemCalabria.Entity.DTO
{
    public class EstoqueOpcoesFiltroDTO
    {
        public List<OpcaoFiltroDTO> TiposPiso { get; set; }
        public List<OpcaoFiltroDTO> Marcas { get; set; }
        public List<OpcaoFiltroDTO> NiveisResistencia { get; set; }
        public List<OpcaoFiltroDTO> Ambientes { get; set; }
        public List<OpcaoFiltroDTO> Acabamentos { get; set; }
    }
}
