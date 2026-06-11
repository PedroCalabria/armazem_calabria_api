namespace ArmazemCalabria.Entity.Entities
{
    public class Usuario: IEntity
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        public bool FlagAprovado { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int? IdUsuarioAlteracao { get; set; } 

        public int IdPerfil { get; set; }
        public Perfil Perfil { get; set; }

        public int? IdUsuarioAprovador { get; set; }
        public Usuario UsuarioAprovador { get; set; }
    }
}
