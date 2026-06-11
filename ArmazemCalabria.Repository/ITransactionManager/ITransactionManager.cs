using System.Data;

namespace ArmazemCalabria.Repository
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionAsync();
        Task RollbackTransactionsAsync();
    }
}
