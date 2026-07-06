namespace ArmazemCalabria.Entity.DTO
{
    /// <summary>
    /// Dicionários de domínio (descrição -> id) usados para resolver os textos da planilha.
    /// As chaves usam comparação case/acento-insensível (ver montagem no repositório).
    /// </summary>
    public class DominiosEstoqueDTO
    {
        public Dictionary<string, int> TiposPiso { get; set; } = [];
        public Dictionary<string, int> Marcas { get; set; } = [];
        public Dictionary<string, int> NiveisResistencia { get; set; } = [];
        public Dictionary<string, int> Acabamentos { get; set; } = [];
        public Dictionary<string, int> Ambientes { get; set; } = [];
    }
}
