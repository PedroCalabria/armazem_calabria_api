namespace ArmazemCalabria.Entity.Helpers.Importacao
{
    /// <summary>
    /// Nomes canônicos das colunas da planilha de estoque (layout de aba única).
    /// A comparação com o cabeçalho da planilha é feita de forma tolerante (sem acento/caixa),
    /// portanto pequenas variações de digitação são aceitas.
    /// </summary>
    public static class ColunasPlanilhaEstoqueHelper
    {
        // Comuns (obrigatórias para todas as linhas)
        public const string Nome = "Nome";
        public const string Cor = "Cor";
        public const string Preco = "Preço";
        public const string Largura = "Largura";
        public const string Comprimento = "Comprimento";
        public const string Espessura = "Espessura";
        public const string Peso = "Peso";
        public const string ResistenteAgua = "Resistente à Água";
        public const string Antiderrapante = "Antiderrapante";
        public const string Quantidade = "Quantidade";
        public const string TipoPiso = "Tipo de Piso";
        public const string Marca = "Marca";
        public const string NivelResistencia = "Nível de Resistência";
        public const string Acabamento = "Acabamento";
        public const string Ambiente = "Ambiente";

        // Específicas por tipo (preenchidas apenas nas linhas do tipo correspondente)
        public const string ClassePei = "Classe PEI";
        public const string Retificado = "Retificado";
        public const string TipoPorcelanato = "Tipo de Porcelanato";
        public const string Acustico = "Acústico";
        public const string TipoInstalacao = "Tipo de Instalação";
        public const string ResistenteCupim = "Resistente a Cupim";
        public const string TipoMadeira = "Tipo de Madeira";
        public const string MadeiraNobre = "Madeira Nobre";
        public const string TipoPedra = "Tipo de Pedra";
        public const string Porosidade = "Porosidade";
        public const string NecessitaImpermeabilizacao = "Necessita Impermeabilização";

        public static readonly string[] Comuns =
        [
            Nome, Cor, Preco, Largura, Comprimento, Espessura, Peso,
            ResistenteAgua, Antiderrapante, Quantidade,
            TipoPiso, Marca, NivelResistencia, Acabamento, Ambiente
        ];

        public static readonly string[] Especificas =
        [
            ClassePei, Retificado, TipoPorcelanato, Acustico, TipoInstalacao,
            ResistenteCupim, TipoMadeira, MadeiraNobre,
            TipoPedra, Porosidade, NecessitaImpermeabilizacao
        ];

        public static IEnumerable<string> TodasEsperadas => Comuns.Concat(Especificas);
    }
}
