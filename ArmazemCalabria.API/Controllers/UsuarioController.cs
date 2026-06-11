using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArmazemCalabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController(IUsuarioBusiness _business) : ControllerBase
    {
        [Authorize(Roles = $"{Constants.PERFIL_GESTOR}")]
        [HttpPost("autorizarPerfilGestor")]
        public async Task AutorizarPerfilGestor(string emailNovoUsuario)
        {
            await _business.AutorizarPerfilGestor(emailNovoUsuario);
        }

        [Authorize(Roles = $"{Constants.PERFIL_GESTOR}")]
        [ProducesResponseType(typeof(List<UsuarioDTO>), StatusCodes.Status201Created)]
        [HttpGet("consultarUsuariosPendentesAprovacao")]
        public async Task<List<UsuarioDTO>> ConsultarUsuariosPendentesAprovacao()
        {
            var usuarios = await _business.ConsultarUsuariosPendentesAprovacao();

            return usuarios;
        }
    }
}
