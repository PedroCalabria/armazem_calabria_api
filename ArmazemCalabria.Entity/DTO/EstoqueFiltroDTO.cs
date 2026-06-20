using ArmazemCalabria.Entity.Enum;

namespace ArmazemCalabria.Entity.DTO
{
    public class EstoqueFiltroDTO
    {
        public List<TiposPiso>? TipoPiso { get; set; }
        public List<MarcasPiso>? Marca { get; set; }
        public List<NiveisResistencia>? NivelResistencia { get; set; }
        public List<TiposAmbiente>? Ambiente { get; set; }
        public List<TiposAcabamento>? Acabamento { get; set; }
    }
}
