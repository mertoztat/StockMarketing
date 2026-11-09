using StockMarkets.DTOs;
using StockMarkets.Helpers;
using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync(QueryObject query);
    Task<Stock?> GetByIdAsync(int id);
    Task<Stock> CreateAsync(Stock stockModel);
    Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateDto);
    Task<Stock?> DeleteAsync(int id);
    Task<bool> StockExist(int id);
    Task<bool> SymbolExistsAsync(string symbol, int? excludeId = null);
}
