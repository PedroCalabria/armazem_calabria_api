using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.Utils.Helpers;
using Microsoft.AspNetCore.Mvc;
using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController(ILoginBusiness _loginBusiness, IConfiguration _configuration) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<RespostaLoginDTO> Login(CredenciaisLoginDTO loginCredentials)
        {
            var response = await _loginBusiness.Login(loginCredentials);
            var expirationTime = int.Parse(_configuration["Authorization:RefreshTokenExpiration"]!);

            Response.SetRefreshTokenCookie(response.RefreshToken!, expirationTime);

            return response;
        }

        [HttpPost("refreshToken")]
        public async Task<RespostaLoginDTO> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _loginBusiness.RefreshToken(refreshToken);
            var expirationTime = int.Parse(_configuration["Authorization:RefreshTokenExpiration"]!);

            Response.SetRefreshTokenCookie(response.RefreshToken!, expirationTime);

            return response;
        }

        [HttpPost("signUp")]
        public async Task SignUp(UsuarioDTO user)
        {
            await _loginBusiness.SignUp(user);
        }
    }
}
