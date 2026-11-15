using System.Security.Claims;

namespace StockMarkets.Extensions;

public static class ClaimsExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.Claims
            .SingleOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
    }
}
