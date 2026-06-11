using Microsoft.AspNetCore.Http;

namespace ArmazemCalabria.Utils.Helpers
{
    public static class AuthenticationHelpers
    {
        public static void SetRefreshTokenCookie(this HttpResponse response, string refreshToken, int expirationDays)
        {
            response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure   = true,
                SameSite = SameSiteMode.Strict,
                Expires  = DateTimeOffset.UtcNow.AddMinutes(expirationDays)
            });
        }
    }
}
