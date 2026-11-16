using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace StockMarkets.Extensions;

public static class ClaimsExtensions
{
    // Raw JWT claim names. Inbound claim mapping is disabled in Program.cs,
    // so the names in the token are exactly the names we read here.
    public const string RoleClaimType = "role";

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
    }

    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;
    }
}
