using ArmazemCalabria.Entity.DTO;
using Microsoft.AspNetCore.Http;

namespace ArmazemCalabria.Business.IBusiness
{
    public interface IImportacaoEstoqueBusiness
    {
        /// <summary>
        /// Valida permissão/arquivo, persiste os metadados + conteúdo e dispara o processamento
        /// através do publicador. Hoje (síncrono) o retorno já traz o resultado do processamento;
        /// na Fase 5 (Kafka) passará a retornar apenas o reconhecimento (status Pendente).
        /// </summary>
        Task<ImportacaoEstoqueResultadoDTO> EnviarPlanilha(IFormFile arquivoEnviado);
    }
}
