using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities.Dominio
{
    public class NivelResistencia : IEntity
    {
        public int IdNivelResistencia { get; set; }
        public string Descricao { get; set; }

        public List<Piso> Pisos { get; set; }
    }
}
