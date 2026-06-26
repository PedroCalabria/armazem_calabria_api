using System.ComponentModel;

namespace ArmazemCalabria.Entity.Enum
{
    public enum StatusPedido
    {
        [Description("Pendente")]
        Pendente = 1,

        [Description("Aprovado")]
        Aprovado = 2,

        [Description("Rejeitado")]
        Rejeitado = 3
    }
}
