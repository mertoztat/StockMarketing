using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface IPortfolioRepository
{
    Task<List<Stock>> GetUserPortfolioAsync(AppUser user);
    Task<bool> ExistsAsync(string appUserId, int stockId);
    Task<Portfolio> CreateAsync(Portfolio portfolio);
    Task<Portfolio?> DeleteAsync(string appUserId, int stockId);
}
