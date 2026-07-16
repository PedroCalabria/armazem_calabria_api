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

        // Sem [TransactionRequired]: o insert do arquivo é persistido/commitado imediatamente
        // (InsertAsync -> SaveChangesAsync) antes de publicar no Kafka, evitando que o consumer
        // leia o evento antes de o registro estar commitado.
        [Authorize(Roles = "Gestor, Logista Interno")]
        [ProducesResponseType(typeof(ImportacaoEstoqueResultadoDTO), StatusCodes.Status200OK)]
        [HttpPost("importarPlanilha")]
        public async Task<ImportacaoEstoqueResultadoDTO> ImportarPlanilha(IFormFile arquivo)
        {
            return await _importacaoBusiness.EnviarPlanilha(arquivo);
        }

        // Consulta de status/resultado usada pelo frontend em polling: a importação é assíncrona
        // (Kafka), então após o envio o cliente consulta este endpoint até o arquivo atingir um
        // estado terminal (Processado / ProcessadoComErros / Falha).
        [Authorize(Roles = "Gestor, Logista Interno")]
        [ProducesResponseType(typeof(ImportacaoEstoqueResultadoDTO), StatusCodes.Status200OK)]
        [HttpGet("importacao/{idArquivo:int}")]
        public async Task<ImportacaoEstoqueResultadoDTO> ConsultarStatusImportacao(int idArquivo)
        {
            return await _importacaoBusiness.ConsultarStatusAsync(idArquivo);
        }

        // Fallback de carga manual (caso o Kafka/consumer esteja indisponível): processa de forma
        // síncrona um arquivo já enviado. Restrito ao Gestor. Reprocessa pedidos pendentes ao final.
        [Authorize(Roles = "Gestor")]
        [TransactionRequired]
        [ProducesResponseType(typeof(ImportacaoEstoqueResultadoDTO), StatusCodes.Status200OK)]
        [HttpPost("processar/{idArquivo:int}")]
        public async Task<ImportacaoEstoqueResultadoDTO> ProcessarManualmente(int idArquivo)
        {
            return await _importacaoBusiness.ProcessarImportacaoAsync(idArquivo);
        }
    }
}
