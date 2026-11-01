using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Mappers;

public static class StockMappers
{
    public static StockDto ToStockDto(this Stock stockModel)
    {
        return new StockDto
        {
            Id = stockModel.Id,
            Symbol = stockModel.Symbol,
            CompanyName = stockModel.CompanyName,
            Purchase = stockModel.Purchase,
            LastDividend = stockModel.LastDividend,
            Industry = stockModel.Industry,
            MarketCap = stockModel.MarketCap
        };
    }
    public static Stock ToStockFromCreateDto(this CreateStockRequestDto stockDto)
    {
        return new Stock
        {
            Symbol = stockDto.Symbol,
            CompanyName = stockDto.CompanyName,
            Purchase = stockDto.Purchase,
            LastDividend = stockDto.LastDividend,
            Industry = stockDto.Industry,
            MarketCap = stockDto.MarketCap
        };
    }
    public static void UpdateFromDto(this Stock stockModel, UpdateStockRequestDto updateDto)
    {
        stockModel.Symbol = updateDto.Symbol;
        stockModel.CompanyName = updateDto.CompanyName;
        stockModel.Purchase = updateDto.Purchase;
        stockModel.LastDividend = updateDto.LastDividend;
        stockModel.Industry = updateDto.Industry;
        stockModel.MarketCap = updateDto.MarketCap;
    }
}