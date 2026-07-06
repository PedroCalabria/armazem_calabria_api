namespace ArmazemCalabria.Entity.DTO
{
    /// <summary>
    /// Representa uma linha bruta lida da planilha de estoque.
    /// Os valores das células são mantidos como texto (por nome de coluna do cabeçalho)
    /// e convertidos/validados na etapa de processamento.
    /// </summary>
    public class LinhaPlanilhaEstoqueDTO
    {
        public int NumeroLinha { get; set; }
        public Dictionary<string, string> Valores { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
