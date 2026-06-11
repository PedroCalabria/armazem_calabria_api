using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.Business.IBusiness
{
    public interface ILoginBusiness
    {
        public Task<RespostaLoginDTO> Login(CredenciaisLoginDTO credenciaisLogin);
        public Task<RespostaLoginDTO> RefreshToken(string? refreshToken);
        public Task SignUp(UsuarioDTO usuario);
    }
}
