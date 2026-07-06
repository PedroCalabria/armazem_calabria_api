using ArmazemCalabria.Entity.Enum;

namespace ArmazemCalabria.Entity.Entities.Importacao
{
    public class ArquivoImportacao : IEntity
    {
        public int IdArquivo { get; set; }
        public string NomeOriginal { get; set; }
        public string ContentType { get; set; }
        public long TamanhoBytes { get; set; }
        public byte[] Conteudo { get; set; }

        public StatusImportacao Status { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalSucesso { get; set; }
        public int TotalErros { get; set; }
        public string? MensagemErro { get; set; }

        public int IdUsuarioEnvio { get; set; }
        public Usuario UsuarioEnvio { get; set; }

        public DateTime DataEnvio { get; set; }
        public DateTime? DataProcessamento { get; set; }

        public ICollection<ErroImportacao> Erros { get; set; } = [];
    }
}
