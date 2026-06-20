namespace ArmazemCalabria.Entity.DTO
{
    public class EstoqueGridItemDTO
    {
        public int IdPiso { get; set; }
        public int IdTipoPiso { get; set; }
        public string Nome { get; set; }
        public string Cor { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string TipoPiso { get; set; }
        public string Marca { get; set; }
        public string NivelResistencia { get; set; }
        public string Ambiente { get; set; }
        public string Acabamento { get; set; }

        public int? ClassePei { get; set; }
        public bool? FlagRetificado { get; set; }
        public string? TipoPorcelanato { get; set; }
        public bool? FlagAcustico { get; set; }
        public string? TipoInstalacao { get; set; }
        public bool? FlagResistenteCupim { get; set; }
        public string? TipoMadeira { get; set; }
        public bool? FlagMadeiraNobre { get; set; }
        public string? TipoPedra { get; set; }
        public bool? FlagPorosidade { get; set; }
        public bool? FlagNecessitaImpermeabilizacao { get; set; }
    }
}
