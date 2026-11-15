using Microsoft.EntityFrameworkCore;
using StockMarkets.Data;
using StockMarkets.DTOs;
using StockMarkets.Helpers;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;
using StockMarkets.Models;

namespace StockMarkets.Repository;

public class StockRepository : IStockRepository
{
    private readonly ApplicationDBContext _context;

    public StockRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<Stock>> GetAllAsync(QueryObject query)
    {
        var stocksQuery = _context.Stocks.Include(c => c.Comments).ThenInclude(c => c.AppUser).AsQueryable();

        if (!string.IsNullOrEmpty(query.Symbol)) stocksQuery = stocksQuery.Where(s => s.Symbol.Contains(query.Symbol));

        if (!string.IsNullOrEmpty(query.CompanyName))
            stocksQuery = stocksQuery.Where(s => s.CompanyName.Contains(query.CompanyName));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
            {
                stocksQuery = query.isDescending
                    ? stocksQuery.OrderByDescending(s => s.Symbol)
                    : stocksQuery.OrderBy(s => s.Symbol);
            }
            else if (query.SortBy.Equals("CompanyName", StringComparison.OrdinalIgnoreCase))
            {
                stocksQuery = query.isDescending
                    ? stocksQuery.OrderByDescending(s => s.CompanyName)
                    : stocksQuery.OrderBy(s => s.CompanyName);
            }
        }

        int skip = (query.PageNumber - 1) * query.PageSize;

        return await stocksQuery.Skip(skip).Take(query.PageSize).ToListAsync();
    }

    public async Task<Stock?> GetByIdAsync(int id)
    {
        return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    {
        return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
    }

    public async Task<Stock> CreateAsync(Stock stockModel)
    {
        _context.Stocks.Add(stockModel);
        await _context.SaveChangesAsync();
        return stockModel;
    }

    public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateDto)
    {
        var existingStock = await _context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == id);
        if (existingStock == null)
        {
            return null;
        }

        existingStock.UpdateFromDto(updateDto);
        await _context.SaveChangesAsync();
        return existingStock;
    }

    public async Task<Stock?> DeleteAsync(int id)
    {
        var stockModel = await _context.Stocks.FindAsync(id);
        if (stockModel == null)
        {
            return null;
        }

        _context.Stocks.Remove(stockModel);
        await _context.SaveChangesAsync();
        return stockModel;
    }

    public Task<bool> StockExist(int id)
    {
        return _context.Stocks.AnyAsync(s => s.Id == id);
    }

    public Task<bool> SymbolExistsAsync(string symbol, int? excludeId = null)
    {
        return _context.Stocks.AnyAsync(s => s.Symbol == symbol && s.Id != excludeId);
    }
}
