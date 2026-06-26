using ArmazemCalabria.Business.IBusiness;
using ArmazemCalabria.CrossCutting.Exceptions;
using ArmazemCalabria.Entity.DTO;
using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Entity.Enum;
using ArmazemCalabria.Repository.IRepository;
using ArmazemCalabria.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArmazemCalabria.Business.Imp.Business
{
    public class LoginBusiness(IUsuarioRepository _repository, IConfiguration _configuration) : ILoginBusiness
    {
        private const string RefreshTokenType = "refresh";

        public async Task<RespostaLoginDTO> Login(CredenciaisLoginDTO credenciaisLogin)
        {
            var usuario = await _repository.ObterUsuarioPorEmail(credenciaisLogin.Email);

            if (usuario is null || !BCrypt.Net.BCrypt.Verify(credenciaisLogin.Senha, usuario.Senha))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            if (!usuario.FlagAprovado)
                throw new BusinessException("Aguardando aprovação de um gestor.");

            return BuildAuthResponse(usuario);
        }

        public async Task<RespostaLoginDTO> RefreshToken(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token não encontrado.");

            var principal = ValidateRefreshToken(refreshToken);
            var idUsuario = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var usuario = await _repository.ObterUsuarioPorId(idUsuario);
            if (usuario is null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            return BuildAuthResponse(usuario);
        }

        public async Task SignUp(UsuarioDTO usuario)
        {
            var dataAtual = DateTime.UtcNow;
            var usuarioExistente = await _repository.ObterUsuarioPorEmail(usuario.Email);
            if (usuarioExistente is not null)
                throw new InvalidOperationException("Já existe um usuário com este email.");

            var novoUsuario = new Usuario
            {
                Email = usuario.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha),
                Nome = usuario.Nome,
                IdPerfil = usuario.IdPerfil,
                DataCriacao = dataAtual,
                FlagAprovado = usuario.IdPerfil != TiposPerfilUsuario.Gestor.AsInt(),
            };

            await _repository.InsertAsync(novoUsuario);
        }

        #region private methods

        private RespostaLoginDTO BuildAuthResponse(Usuario usuario)
        {
            return new RespostaLoginDTO
            {
                Token = GenerateToken(usuario, isRefresh: false),
                RefreshToken = GenerateToken(usuario, isRefresh: true),
                Usuario = new UsuarioDTO
                {
                    Email = usuario.Email,
                    Nome = usuario.Nome,
                    IdPerfil = usuario.IdPerfil,
                }
            };
        }

        private string GenerateToken(Usuario usuario, bool isRefresh)
        {
            var secretKey = _configuration["Authorization:SecretKey"]!;
            var issuer = _configuration["Authorization:Issuer"]!;
            var audience = _configuration["Authorization:Audience"]!;
            var expiration = isRefresh
                ? DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Authorization:RefreshTokenExpiration"]!))
                : DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Authorization:AccessTokenExpiration"]!));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = isRefresh
                ?
                [
                    new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                    new("type",                      RefreshTokenType),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]
                : new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub,   usuario.IdUsuario.ToString()),
                    new(JwtRegisteredClaimNames.Email, usuario.Email),
                    new(JwtRegisteredClaimNames.Name,  usuario.Nome),
                    new("roles",                       usuario.Perfil.Descricao),
                    new(ClaimTypes.Role,               usuario.Perfil.Descricao),
                    new("idRole",                      usuario.Perfil.IdPerfil.ToString()),
                    new("type",                        "access"),
                    new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
                };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            var secretKey = _configuration["Authorization:SecretKey"]!;
            var issuer = _configuration["Authorization:Issuer"]!;
            var audience = _configuration["Authorization:Audience"]!;

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(refreshToken, validationParams, out _);

                var tokenType = principal.FindFirst("type")?.Value;
                if (tokenType != RefreshTokenType)
                    throw new UnauthorizedAccessException("Token inválido.");

                return principal;
            }
            catch (SecurityTokenException)
            {
                throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");
            }
        }

        #endregion
    }
}
