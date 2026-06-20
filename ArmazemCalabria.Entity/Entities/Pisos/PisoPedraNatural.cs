namespace ArmazemCalabria.Entity.Entities.Pisos
{
    public class PisoPedraNatural : Piso
    {
        public string TipoPedra { get; set; }
        public bool FlagPorosidade { get; set; }
        public bool FlagNecessitaImpermeabilizacao { get; set; }
    }
}
