using Microsoft.AspNetCore.Http;

namespace ArmazemCalabria.Utils.Helpers
{
    public static class AuthenticationHelpers
    {
        private static CookieOptions RefreshTokenCookieOptions => new()
        {
            HttpOnly = true,
            Secure   = true,
            SameSite = SameSiteMode.Strict,
        };

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

        public static void ClearRefreshTokenCookie(this HttpResponse response)
        {
            response.Cookies.Delete("refreshToken", RefreshTokenCookieOptions);
        }
    }
}
