using Microsoft.AspNetCore.Identity;

namespace StockMarkets.Models;

public class AppUser : IdentityUser
{
    public List<Portfolio> Portfolios { get; set; }

}
