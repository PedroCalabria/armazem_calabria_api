using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities.Dominio
{
    public class Acabamento : IEntity
    {
        public int IdAcabamento { get; set; }
        public string Descricao { get; set; }

        public List<Piso> Pisos { get; set; }
    }
}
