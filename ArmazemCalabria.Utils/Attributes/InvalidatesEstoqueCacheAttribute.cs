namespace ArmazemCalabria.Utils.Attributes
{
    /// <summary>
    /// Marca endpoints que alteram o estoque (Fase 6). O ApiMiddleware, após o commit
    /// da transação (caminho de sucesso), invalida o cache de estoque no Redis.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class InvalidatesEstoqueCacheAttribute : Attribute
    {
    }
}
