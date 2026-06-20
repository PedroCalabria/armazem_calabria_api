using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities.Dominio
{
    public class Ambiente : IEntity
    {
        public int IdAmbiente { get; set; }
        public string Descricao { get; set; }

        public List<Piso> Pisos { get; set; }
    }
}
