namespace ArmazemCalabria.Entity.Entities.Importacao
{
    public class ErroImportacao : IEntity
    {
        public int IdErro { get; set; }

        public int IdArquivo { get; set; }
        public ArquivoImportacao Arquivo { get; set; }

        public int NumeroLinha { get; set; }
        public string? Coluna { get; set; }
        public string Mensagem { get; set; }
        public string? ConteudoLinha { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
