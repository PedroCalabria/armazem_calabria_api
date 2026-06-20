using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities.Dominio
{
    public class TipoPiso : IEntity
    {
        public int IdTipoPiso { get; set; }
        public string Descricao { get; set; }

        public List<Piso> Pisos { get; set; }
    }
}
