using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync();
    Task<Stock?> GetByIdAsync(int id);
    Task<Stock> CreateAsync(Stock stockModel);
    Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateDto);
    Task<Stock?> DeleteAsync(int id);
}