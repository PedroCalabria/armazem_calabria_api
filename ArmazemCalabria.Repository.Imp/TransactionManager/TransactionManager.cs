using System.Data;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository.Imp.TransactionManager
{
    public class TransactionManager(Context context) : ITransactionManager
    {
        private readonly Context _context = context;

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            var activeTransaction = _context.Database.CurrentTransaction;
            if (activeTransaction == null)
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var transaction = await connection.BeginTransactionAsync(isolationLevel);
                await _context.Database.UseTransactionAsync(transaction);
            }
        }

        public async Task CommitTransactionAsync()
        {
            var contextHasChanges = _context.ChangeTracker.HasChanges();

            if (contextHasChanges)
            {
                await _context.SaveChangesAsync();
            }

            var activeTransaction = _context.Database.CurrentTransaction;

            await activeTransaction.CommitAsync();
            await activeTransaction.DisposeAsync();
        }

        public async Task RollbackTransactionsAsync()
        {
            var activeTransaction = _context.Database.CurrentTransaction;

            if (activeTransaction != null)
            {
                await activeTransaction.RollbackAsync();
                await activeTransaction.DisposeAsync();
            }
        }
    }
}
