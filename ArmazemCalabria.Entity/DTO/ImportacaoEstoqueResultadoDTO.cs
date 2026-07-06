namespace ArmazemCalabria.Entity.DTO
{
    public class ImportacaoEstoqueResultadoDTO
    {
        public int IdArquivo { get; set; }
        public int Status { get; set; }
        public string StatusDescricao { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalSucesso { get; set; }
        public int TotalInseridos { get; set; }
        public int TotalAtualizados { get; set; }
        public int TotalErros { get; set; }
        public string? MensagemErro { get; set; }
        public List<ErroImportacaoDTO> Erros { get; set; } = [];
    }
}
