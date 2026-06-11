using System.ComponentModel;

namespace ArmazemCalabria.Entity.Enum
{
    public enum TiposPerfilUsuario
    {
        [Description ("Gestor")]
        Gestor = 1,
        [Description ("Logista Interno")]
        LogistaInterno,
        [Description ("Logista Externo")]
        LogistaExterno
    }
}
