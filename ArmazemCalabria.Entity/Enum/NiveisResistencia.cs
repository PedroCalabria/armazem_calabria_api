using System.ComponentModel;

namespace ArmazemCalabria.Entity.Enum
{
    public enum NiveisResistencia
    {
        [Description("Alta")]
        Alta = 1,
        [Description("Média")]
        Media,
        [Description("Baixa")]
        Baixa,
        [Description("Tráfego Comercial")]
        TrafegoComercial,
        [Description("Tráfego Residencial")]
        TrafegoResidencial
    }
}
