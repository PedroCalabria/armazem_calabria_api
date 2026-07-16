using ArmazemCalabria.API.Messaging;
using ArmazemCalabria.Business.Imp.Messaging;
using ArmazemCalabria.Business.IMessaging;
using ArmazemCalabria.CrossCutting.Configurations;

namespace ArmazemCalabria.API.Configurations
{
    public static class KafkaConfiguration
    {
        public static void AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(KafkaSettings.SectionName)
                .Get<KafkaSettings>() ?? new KafkaSettings();

            // Instância única compartilhada entre producer (Business.Imp) e consumer (API).
            services.AddSingleton(settings);

            // Producer: registrado manualmente (o auto-registro por sufixo cobre apenas Business/Repository).
            services.AddSingleton<IImportacaoEventPublisher, KafkaImportacaoEventPublisher>();

            // Consumer: BackgroundService que roda no mesmo processo da API.
            services.AddHostedService<ImportacaoConsumerBackgroundService>();
        }
    }
}
