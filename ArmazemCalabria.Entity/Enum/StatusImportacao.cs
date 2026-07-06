using System.ComponentModel;

namespace ArmazemCalabria.Entity.Enum
{
    public enum StatusImportacao
    {
        [Description("Pendente")]
        Pendente = 1,
        [Description("Processando")]
        Processando = 2,
        [Description("Processado")]
        Processado = 3,
        [Description("Processado com erros")]
        ProcessadoComErros = 4,
        [Description("Falha")]
        Falha = 5
    }
}
