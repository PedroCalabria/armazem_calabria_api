namespace ArmazemCalabria.Entity.DTO
{
    /// <summary>
    /// Dados do arquivo enviado, já desacoplados do tipo IFormFile da camada web.
    /// </summary>
    public class UploadPlanilhaDTO
    {
        public string NomeOriginal { get; set; }
        public string ContentType { get; set; }
        public long TamanhoBytes { get; set; }
        public byte[] Conteudo { get; set; }
    }
}
