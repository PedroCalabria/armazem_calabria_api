namespace ArmazemCalabria.CrossCutting.Configurations
{
    /// <summary>
    /// Configuração do Kafka lida da seção "Kafka" do appsettings.json.
    /// Vive no CrossCutting para ser compartilhada entre o producer (Business.Imp) e o consumer (API).
    /// </summary>
    public class KafkaSettings
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; set; } = "localhost:9092";
        public string Topic { get; set; } = "armazem.estoque.importacao";
        public string ConsumerGroupId { get; set; } = "armazem-consumer";
    }
}
