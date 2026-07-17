using ArmazemCalabria.Business.ICache;
using ArmazemCalabria.Business.Imp.Cache;
using ArmazemCalabria.CrossCutting.Configurations;
using StackExchange.Redis;

namespace ArmazemCalabria.API.Configurations
{
    public static class RedisConfiguration
    {
        public static void AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(RedisSettings.SectionName)
                .Get<RedisSettings>() ?? new RedisSettings();

            services.AddSingleton(settings);

            // Conexão única e compartilhada (recomendação do StackExchange.Redis).
            // AbortOnConnectFail = false: a API sobe mesmo se o Redis estiver indisponível;
            // o cache degrada para o banco (ver EstoqueCache).
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(settings.ConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });

            // Registro manual (o auto-registro por sufixo cobre apenas Business/Repository).
            services.AddSingleton<IEstoqueCache, EstoqueCache>();
        }
    }
}
