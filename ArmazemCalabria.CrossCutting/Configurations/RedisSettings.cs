namespace ArmazemCalabria.CrossCutting.Configurations
{
    /// <summary>
    /// Configuração do Redis lida da seção "Redis" do appsettings.json (Fase 6).
    /// Vive no CrossCutting para ser compartilhada entre a implementação do cache
    /// (Business.Imp) e o registro de DI (API).
    /// </summary>
    public class RedisSettings
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>Prefixo aplicado a todas as chaves de cache (ex.: "armazem").</summary>
        public string InstanceName { get; set; } = "armazem";

        /// <summary>Tempo de vida das entradas de cache, em segundos.</summary>
        public int TtlSegundos { get; set; } = 600;
    }
}
