using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArmazemCalabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController(
        IEstoqueBusiness _business,
        IImportacaoEstoqueBusiness _importacaoBusiness) : ControllerBase
    {
        [Authorize]
        [ProducesResponseType(typeof(List<EstoqueGridItemDTO>), StatusCodes.Status200OK)]
        [HttpGet("consultarEstoque")]
        public async Task<List<EstoqueGridItemDTO>> ConsultarEstoque([FromQuery] EstoqueFiltroDTO filtro)
        {
            return await _business.ConsultarEstoque(filtro);
        }

        [Authorize]
        [ProducesResponseType(typeof(EstoqueOpcoesFiltroDTO), StatusCodes.Status200OK)]
        [HttpGet("obterOpcoesFiltro")]
        public async Task<EstoqueOpcoesFiltroDTO> ObterOpcoesFiltro()
        {
            return await _business.ObterOpcoesFiltro();
        }

        [Authorize(Roles = "Gestor, Logista Interno")]
        [TransactionRequired]
        [ProducesResponseType(typeof(ImportacaoEstoqueResultadoDTO), StatusCodes.Status200OK)]
        [HttpPost("importarPlanilha")]
        public async Task<ImportacaoEstoqueResultadoDTO> ImportarPlanilha(IFormFile arquivo)
        {
            return await _importacaoBusiness.EnviarPlanilha(arquivo);
        }
    }
}
