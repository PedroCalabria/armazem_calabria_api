namespace ArmazemCalabria.Entity.DTO
{
    /// <summary>
    /// Resultado da leitura/validação de layout da planilha de estoque.
    /// Quando <see cref="LayoutValido"/> é falso, <see cref="MensagemLayout"/> descreve o problema
    /// e o arquivo deve ser rejeitado por inteiro.
    /// </summary>
    public class ResultadoLeituraPlanilhaDTO
    {
        public bool LayoutValido { get; set; }
        public string? MensagemLayout { get; set; }
        public List<LinhaPlanilhaEstoqueDTO> Linhas { get; set; } = [];
    }
}
