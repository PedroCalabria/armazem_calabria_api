using ArmazemCalabria.CrossCutting.Extensions;
using ArmazemCalabria.CrossCutting;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;

namespace ArmazemCalabria.API.Middleware
{
    public class UserContextMiddleware(IUserContext userContext) : IMiddleware
    {
        private readonly IUserContext _userContext = userContext;

        public async Task<bool> AddUser(HttpContext context)
        {
            bool result;

            if (VerifyAuthenticatedUser(context))
            {
                var securityToken = GetSecurityToken(context);

                if (securityToken != null)
                {
                    PopulateUserContext(context, securityToken);
                }

                result = true;
            }
            else
            {
                throw new UnauthorizedAccessException("Token inválido ou expirado.");
            }

            return await Task.Run(() => result);
        }

        private static JwtSecurityToken GetSecurityToken(HttpContext context)
        {
            var authToken = context.Request.Headers.Authorization.ToString();

            if (authToken.Trim().Length > 0)
            {
                var token = authToken.Replace("Bearer", string.Empty).Trim();
                return new JwtSecurityTokenHandler().ReadJwtToken(token);
            }

            return null;
        }

        private void PopulateUserContext(HttpContext context, JwtSecurityToken securityToken)
        {
            _userContext.ResquestId = Guid.NewGuid();
            _userContext.StartDateTime = DateTime.UtcNow;
            _userContext.SourceInfo = new SourceInfo
            {
                IP = context?.Connection?.RemoteIpAddress,
                Data = GetAllHeaders(context)
            };

            if (securityToken != null && securityToken.Claims.Any())
            {
                var userId = securityToken.Claims.GetClaimValue("sub");
                var name = securityToken.Claims.GetClaimValue("name");
                var email = securityToken.Claims.GetClaimValue("email");
                var roles = securityToken.Claims.GetClaimValue("roles");
                var idPerfil = securityToken.Claims.GetClaimValue("idRole");

                _userContext.AddData("UserId", userId);
                _userContext.AddData("name", name);
                _userContext.AddData("email", email);
                _userContext.AddData("roles", roles);
                _userContext.AddData("IdPerfil", idPerfil);
            }
        }

        private static bool VerifyAuthenticatedUser(HttpContext context)
        {
            var authToken = context.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(authToken))
            {
                return context.User?.Identity?.IsAuthenticated ?? false;
            }

            return true;
        }

        private static Hashtable GetAllHeaders(HttpContext context)
        {
            var hashtable = new Hashtable();
            var requestHeaders = context.Request.Headers;

            if (requestHeaders == null)
            {
                return hashtable;
            }

            foreach (var header in requestHeaders)
            {
                hashtable.Add(header.Key, header.Value);
            }

            return hashtable;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var _answer = await AddUser(context);

                if (_answer)
                {
                    await next.Invoke(context);
                }
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
    }
}