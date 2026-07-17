using ArmazemCalabria.Business.ICache;
using ArmazemCalabria.CrossCutting.Configurations;
using ArmazemCalabria.Entity.DTO;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace ArmazemCalabria.Business.Imp.Cache
{
    /// <summary>
    /// Implementação Redis do cache de estoque (Fase 6) com estratégia de chave versionada.
    /// As falhas de Redis são tratadas com degradação graciosa: a consulta cai para o banco
    /// (via <c>carregar</c>) em vez de derrubar a requisição.
    /// </summary>
    public class EstoqueCache(
        IConnectionMultiplexer _connection,
        RedisSettings _settings,
        ILogger<EstoqueCache> _logger) : IEstoqueCache
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private string VersaoKey => $"{_settings.InstanceName}:estoque:version";

        public async Task<List<EstoqueGridItemDTO>> ObterOuGravarAsync(
            EstoqueFiltroConsultaDTO filtro,
            Func<Task<List<EstoqueGridItemDTO>>> carregar)
        {
            IDatabase? db = ObterDatabase();

            // Redis indisponível: degrada para o banco.
            if (db is null)
                return await carregar();

            string chave;

            try
            {
                var versao = await db.StringGetAsync(VersaoKey);
                chave = MontarChave(versao.HasValue ? versao.ToString() : "0", filtro);

                var cacheado = await db.StringGetAsync(chave);
                if (cacheado.HasValue)
                {
                    var dados = JsonSerializer.Deserialize<List<EstoqueGridItemDTO>>(cacheado!, JsonOptions);
                    if (dados is not null)
                        return dados;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao ler o cache de estoque no Redis. Consultando o banco.");
                return await carregar();
            }

            // Cache miss: consulta o banco e tenta gravar (falha na gravação não quebra a consulta).
            var resultado = await carregar();

            try
            {
                var json = JsonSerializer.Serialize(resultado, JsonOptions);
                await db.StringSetAsync(chave, json, TimeSpan.FromSeconds(_settings.TtlSegundos));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao gravar o cache de estoque no Redis.");
            }

            return resultado;
        }

        public async Task InvalidarAsync()
        {
            IDatabase? db = ObterDatabase();

            if (db is null)
                return;

            try
            {
                // Incremento atômico: aposenta todas as chaves da versão anterior de uma vez.
                var novaVersao = await db.StringIncrementAsync(VersaoKey);
                _logger.LogInformation("Cache de estoque invalidado. Nova versão: {Versao}.", novaVersao);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao invalidar o cache de estoque no Redis.");
            }
        }

        private IDatabase? ObterDatabase()
        {
            if (!_connection.IsConnected)
            {
                _logger.LogWarning("Redis indisponível (sem conexão). Operação de cache ignorada.");
                return null;
            }

            return _connection.GetDatabase();
        }

        /// <summary>
        /// Monta a chave de cache embutindo a versão e uma serialização determinística do filtro
        /// (cada lista de ids é ordenada, garantindo a mesma chave para filtros equivalentes).
        /// </summary>
        private string MontarChave(string versao, EstoqueFiltroConsultaDTO filtro)
        {
            var partes = string.Join("|",
                Serializar(filtro.IdsTipoPiso),
                Serializar(filtro.IdsMarca),
                Serializar(filtro.IdsNivelResistencia),
                Serializar(filtro.IdsAmbiente),
                Serializar(filtro.IdsAcabamento));

            return $"{_settings.InstanceName}:estoque:v{versao}:{partes}";
        }

        private static string Serializar(List<int> ids)
            => ids.Count == 0 ? "-" : string.Join(",", ids.OrderBy(id => id));
    }
}
