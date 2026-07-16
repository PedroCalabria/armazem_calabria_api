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

        /// <summary>
        /// Processa um arquivo já persistido (leitura + bulk insert) e reprocessa os pedidos pendentes.
        /// Usado pelo consumer Kafka (fluxo assíncrono) e pelo endpoint de carga manual (fallback do Gestor).
        /// </summary>
        Task<ImportacaoEstoqueResultadoDTO> ProcessarImportacaoAsync(int idArquivo);

        /// <summary>
        /// Consulta o status/resultado corrente de um arquivo de importação (para polling do frontend
        /// enquanto o consumer processa em background). Retorna os erros apenas quando o arquivo já
        /// atingiu um estado terminal com erros/falha.
        /// </summary>
        Task<ImportacaoEstoqueResultadoDTO> ConsultarStatusAsync(int idArquivo);
    }
}
