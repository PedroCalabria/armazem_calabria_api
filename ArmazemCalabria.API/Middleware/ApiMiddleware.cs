using ArmazemCalabria.Business.ICache;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Repository;
using ArmazemCalabria.Utils.Attributes;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace ArmazemCalabria.API.Middleware
{
    public class ApiMiddleware(ITransactionManager transactionManager, IEstoqueCache estoqueCache) : IMiddleware
    {
        private readonly ITransactionManager _transactionManager = transactionManager;
        private readonly IEstoqueCache _estoqueCache = estoqueCache;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var endpointMetadata = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata;
            var transactionRequired = endpointMetadata?.GetMetadata<TransactionRequiredAttribute>();
            var invalidatesCache = endpointMetadata?.GetMetadata<InvalidatesEstoqueCacheAttribute>();
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

                // Invalida o cache de estoque APÓS o commit (apenas no caminho de sucesso).
                if (invalidatesCache != null)
                    await _estoqueCache.InvalidarAsync();

                stopwatch.Stop();
            }
            catch (Exception ex)
             {
                if (transactionRequired != null)
                    await _transactionManager.RollbackTransactionsAsync();

                stopwatch.Stop();

                context.Response.StatusCode = ex switch
                {
                    BusinessException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
        }
    }
}
