using System.Security.Claims;

namespace ArmazemCalabria.CrossCutting.Extensions
{
    public static class ClaimsExtensions
    {
        public static List<string> GetValuesOfType(this IEnumerable<Claim> claims, string type)
        {
            return [.. claims.Where(x => x.Type == type).Select(x => x.Value).Distinct()];
        }

        public static string GetClaimValue(this IEnumerable<Claim> claims, string claimType)
        {
            return claims.GetValuesOfType(claimType).FirstOrDefault();
        }
    }
}