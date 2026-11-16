using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user, IEnumerable<string> roles);
}
