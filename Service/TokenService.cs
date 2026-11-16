using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using StockMarkets.Extensions;
using StockMarkets.Interfaces;
using StockMarkets.Models;

namespace StockMarkets.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"] ??
                                                               throw new InvalidOperationException(
                                                                   "JWT Key is not configured.")));
    }

    public string CreateToken(AppUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            // "sub" is the stable identity; the username can change, the Id cannot.
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            // Kept for backward compatibility (CommentController still reads it until TASK-010).
            new(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty)
        };

        // One "role" claim per role; [Authorize(Roles = "...")] reads these.
        claims.AddRange(roles.Select(role => new Claim(ClaimsExtensions.RoleClaimType, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }
};
