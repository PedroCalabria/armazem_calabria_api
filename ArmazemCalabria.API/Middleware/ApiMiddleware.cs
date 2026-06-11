using ArmazemCalabria.Repository;
using ArmazemCalabria.Utils.Attributes;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace ArmazemCalabria.API.Middleware
{
    public class ApiMiddleware(ITransactionManager transactionManager) : IMiddleware
    {
        private readonly ITransactionManager _transactionManager = transactionManager;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var transactionRequired = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<TransactionRequiredAttribute>();
            try
            {
                if (transactionRequired != null)
                {
                    await _transactionManager.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

                    await next.Invoke(context);

                    await _transactionManager.CommitTransactionAsync();
                }
                else
                {
                    await next.Invoke(context);
                }

                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                if (transactionRequired != null)
                    await _transactionManager.RollbackTransactionsAsync();

                stopwatch.Stop();

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
        }
    }
}
