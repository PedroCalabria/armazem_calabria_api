using ArmazemCalabria.Entity.DTO;

namespace ArmazemCalabria.Business.ICache
{
    /// <summary>
    /// Cache (Redis) da consulta de estoque (Fase 6). Usa estratégia de chave versionada:
    /// toda chave embute uma versão global; a invalidação apenas incrementa essa versão,
    /// aposentando de uma vez todas as entradas anteriores (que expiram por TTL).
    /// </summary>
    public interface IEstoqueCache
    {
        /// <summary>
        /// Retorna a consulta cacheada para o filtro informado (cache hit) ou executa
        /// <paramref name="carregar"/>, grava o resultado e o retorna (cache miss).
        /// </summary>
        Task<List<EstoqueGridItemDTO>> ObterOuGravarAsync(
            EstoqueFiltroConsultaDTO filtro,
            Func<Task<List<EstoqueGridItemDTO>>> carregar);

        /// <summary>
        /// Invalida todo o cache de estoque incrementando a versão global. Deve ser
        /// chamado após o commit de qualquer operação que altere o estoque.
        /// </summary>
        Task InvalidarAsync();
    }
}
