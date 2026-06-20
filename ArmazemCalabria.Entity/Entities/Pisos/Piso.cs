using ArmazemCalabria.Entity.Entities.Dominio;

namespace ArmazemCalabria.Entity.Entities.Pisos
{
    public abstract class Piso : IEntity
    {
        public int IdPiso { get; set; }
        public string Nome { get; set; }
        public string Cor { get; set; }
        public decimal Preco { get; set; }
        public decimal Largura { get; set; }
        public decimal Comprimento { get; set; }
        public decimal Espessura { get; set; }
        public decimal Peso { get; set; }
        public bool FlagResistenteAgua { get; set; }
        public bool FlagAntiderrapante { get; set; }
        public int QuantidadeDisponivel { get; set; }

        public int IdTipoPiso { get; set; }
        public TipoPiso TipoPiso { get; set; }
        public int IdMarca { get; set; }
        public Marca Marca { get; set; }
        public int IdNivelResistencia { get; set; }
        public NivelResistencia NivelResistencia { get; set; }
        public int IdAcabamento { get; set; }
        public Acabamento Acabamento { get; set; }
        public int IdAmbiente { get; set; }
        public Ambiente Ambiente { get; set; }

        public DateTime DataCriacao { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public Usuario UsuarioCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int? IdUsuarioAlteracao { get; set; }
        public Usuario UsuarioAlteracao { get; set; }
    }
}
