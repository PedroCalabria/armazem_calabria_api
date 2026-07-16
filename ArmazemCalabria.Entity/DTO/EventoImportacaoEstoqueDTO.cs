namespace ArmazemCalabria.Entity.DTO
{
    /// <summary>
    /// Evento publicado no Kafka após o upload de uma planilha de estoque.
    /// O consumer usa o IdArquivo para localizar o arquivo persistido e processá-lo.
    /// </summary>
    public class EventoImportacaoEstoqueDTO
    {
        public int IdArquivo { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}
