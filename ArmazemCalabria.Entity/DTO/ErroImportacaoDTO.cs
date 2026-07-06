namespace ArmazemCalabria.Entity.DTO
{
    public class ErroImportacaoDTO
    {
        public int NumeroLinha { get; set; }
        public string? Coluna { get; set; }
        public string Mensagem { get; set; }
    }
}
