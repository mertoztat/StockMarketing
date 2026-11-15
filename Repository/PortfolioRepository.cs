using Microsoft.EntityFrameworkCore;
using StockMarkets.Data;
using StockMarkets.Interfaces;
using StockMarkets.Models;

namespace StockMarkets.Repository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDBContext _context;

    public PortfolioRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<Stock>> GetUserPortfolioAsync(AppUser user)
    {
        return await _context.Portfolios
            .AsNoTracking()
            .Where(p => p.AppUserId == user.Id)
            .Select(p => new Stock
            {
                Id = p.Stock.Id,
                Symbol = p.Stock.Symbol,
                CompanyName = p.Stock.CompanyName,
                Industry = p.Stock.Industry,
                MarketCap = p.Stock.MarketCap,
                Purchase = p.Stock.Purchase,
                LastDividend = p.Stock.LastDividend
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string appUserId, int stockId)
    {
        return await _context.Portfolios
            .AnyAsync(p => p.AppUserId == appUserId && p.StockId == stockId);
    }

    public async Task<Portfolio> CreateAsync(Portfolio portfolio)
    {
        await _context.Portfolios.AddAsync(portfolio);
        await _context.SaveChangesAsync();
        return portfolio;
    }

    public async Task<Portfolio?> DeleteAsync(string appUserId, int stockId)
    {
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == appUserId && p.StockId == stockId);

        if (portfolio is null) return null;

        _context.Portfolios.Remove(portfolio);
        await _context.SaveChangesAsync();
        return portfolio;
    }
}
