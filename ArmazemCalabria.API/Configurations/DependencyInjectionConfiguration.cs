using ArmazemCalabria.API.Middleware;
using ArmazemCalabria.Business.Imp.Business;
using ArmazemCalabria.CrossCutting;
using ArmazemCalabria.Repository;
using ArmazemCalabria.Repository.Imp.Repository;
using ArmazemCalabria.Repository.Imp.TransactionManager;
using System.Reflection;

namespace ArmazemCalabria.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            InjectMiddlewares(services);

            RegisterGeneric(services, typeof(LoginBusiness).Assembly, "Business");
            RegisterGeneric(services, typeof(UsuarioRepository).Assembly, "Repository");

            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IUserContext, UserContext>();
        }

        private static void InjectMiddlewares(IServiceCollection services)
        {
            services.AddTransient<UserContextMiddleware>();
            services.AddTransient<ApiMiddleware>();
        }

        private static void RegisterGeneric(IServiceCollection services, Assembly assembly, string sufixo)
        {
            var implementacoes = assembly.GetTypes()
                                .Where(t => t.IsClass && t.Name.EndsWith(sufixo));

            foreach (var implementacao in implementacoes)
            {
                var interfaceType = Array.Find(implementacao.GetInterfaces(), i => i.Name.EndsWith(implementacao.Name));

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementacao);
                }
            }
        }
    }
}
