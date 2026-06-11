using System.Text.Json.Serialization;

namespace ArmazemCalabria.Entity.DTO
{
    public class RespostaLoginDTO
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioDTO Usuario { get; set; } = new UsuarioDTO();

        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
}
