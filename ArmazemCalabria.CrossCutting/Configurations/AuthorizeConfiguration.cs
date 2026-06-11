using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using tusdotnet.Helpers;

namespace ArmazemCalabria.CrossCutting.Configurations
{
    public static class AuthorizeConfiguration
    {
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            string[] origemPermitida = [configuration.GetValue<string>("Authentication:AcceptedOrigin")!];
            string[] cabecalhosPermitidos = ["authorization", "x-requested-with", "x-signalr-user-agent", "Content-Type"];

            Console.WriteLine($"Origens lidas da configuração: {string.Join(",", origemPermitida)}");
            Console.WriteLine($"Lendo origens fixos: {origemPermitida is null || origemPermitida.Length == 0 || origemPermitida[0] == null}");

            if (origemPermitida is null || origemPermitida.Length == 0 || origemPermitida[0] == null)
            {
                origemPermitida = [
                    "http://localhost:5173",
                ];
            }

            services.AddCors(o => o.AddPolicy("CORS_POLICY", builder =>
            {
                builder.WithOrigins(origemPermitida)
                       .WithHeaders(cabecalhosPermitidos)
                       .AllowAnyMethod()
                       .WithExposedHeaders(CorsHelper.GetExposedHeaders());
            }));
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opcoes =>
            {
                opcoes.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Authorization:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Authorization:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Authorization:SecretKey"]!)
                    ),
                    RoleClaimType = ClaimsIdentity.DefaultRoleClaimType,
                    ClockSkew = TimeSpan.Zero
                };

                opcoes.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var headerToken = context.Request.Headers.Authorization.ToString();
                        var queryToken  = context.Request.Query["access_token"].ToString();

                        Console.WriteLine($"[JWT] Header Authorization: {(string.IsNullOrEmpty(headerToken) ? "AUSENTE" : "PRESENTE")}");
                        Console.WriteLine($"[JWT] Query access_token:   {(string.IsNullOrEmpty(queryToken)  ? "AUSENTE" : "PRESENTE")}");

                        if (!string.IsNullOrEmpty(queryToken))
                            context.Token = queryToken;

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"[JWT] Token válido. User: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT] Falha na autenticação: {context.Exception.GetType().Name} - {context.Exception.Message}");
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        Console.WriteLine($"[JWT] Challenge disparado. Erro: {context.Error} - {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }
    }
}