namespace ArmazemCalabria.Business.IMessaging
{
    /// <summary>
    /// Abstração do publicador de eventos de importação de estoque.
    /// A implementação (Kafka) publica um evento para que o consumer processe o arquivo de forma assíncrona.
    /// </summary>
    public interface IImportacaoEventPublisher
    {
        Task PublicarImportacaoAsync(int idArquivo);
    }
}
