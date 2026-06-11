namespace ArmazemCalabria.Entity.Entities
{
    public class Perfil : IEntity
    {
        public int IdPerfil { get; set; }
        public string Descricao { get; set; }

        public List<Usuario> Usuarios { get; set; }
    }
}