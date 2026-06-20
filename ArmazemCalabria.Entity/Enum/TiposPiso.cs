using System.ComponentModel;

namespace ArmazemCalabria.Entity.Enum
{
    public enum TiposPiso
    {
        [Description("Cerâmica")]
        Ceramica = 1,
        [Description("Porcelanato")]
        Porcelanato,
        [Description("Vinílico")]
        Vinilico,
        [Description("Laminado")]
        Laminado,
        [Description("Madeira")]
        Madeira,
        [Description("Pedra Natural")]
        PedraNatural,
        [Description("Cimento Queimado")]
        CimentoQueimado
    }
}
