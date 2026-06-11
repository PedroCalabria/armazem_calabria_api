using System.Text.Json.Serialization;

namespace ArmazemCalabria.Entity.DTO
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int IdPerfil { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
