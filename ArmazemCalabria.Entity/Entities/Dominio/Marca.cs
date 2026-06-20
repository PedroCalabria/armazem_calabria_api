using ArmazemCalabria.Entity.Entities.Pisos;

namespace ArmazemCalabria.Entity.Entities.Dominio
{
    public class Marca : IEntity
    {
        public int IdMarca { get; set; }
        public string Nome { get; set; }

        public List<Piso> Pisos { get; set; }
    }
}
