using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArmazemCalabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController(IPedidoBusiness _business) : ControllerBase
    {
        [Authorize]
        [ProducesResponseType(typeof(List<PedidoGridItemDTO>), StatusCodes.Status200OK)]
        [HttpGet("consultarPedidos")]
        public async Task<List<PedidoGridItemDTO>> ConsultarPedidos()
        {
            return await _business.ConsultarPedidos();
        }

        [Authorize]
        [TransactionRequired]
        [ProducesResponseType(typeof(PedidoCriadoDTO), StatusCodes.Status200OK)]
        [HttpPost("solicitarPedido")]
        public async Task<PedidoCriadoDTO> SolicitarPedido([FromBody] SolicitarPedidoDTO dto)
        {
            return await _business.SolicitarPedido(dto);
        }

        [Authorize]
        [TransactionRequired]
        [HttpPost("aprovarPedido")]
        public async Task AprovarPedido([FromQuery] int idPedido)
        {
            await _business.AprovarPedido(idPedido);
        }

        [Authorize]
        [TransactionRequired]
        [ProducesResponseType(typeof(AprovarPedidosResultadoDTO), StatusCodes.Status200OK)]
        [HttpPost("aprovarPedidos")]
        public async Task<AprovarPedidosResultadoDTO> AprovarPedidos([FromBody] AprovarPedidosDTO dto)
        {
            return await _business.AprovarPedidos(dto);
        }

        [Authorize]
        [TransactionRequired]
        [HttpPost("rejeitarPedido")]
        public async Task RejeitarPedido([FromBody] RejeitarPedidoDTO dto)
        {
            await _business.RejeitarPedido(dto);
        }
    }
}
