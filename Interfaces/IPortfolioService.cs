using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface IPortfolioService
{
    /// <exception cref="Exceptions.UnauthenticatedException">The user id does not belong to an existing user.</exception>
    Task<List<Stock>> GetUserPortfolioAsync(string userId);

    /// <exception cref="Exceptions.UnauthenticatedException">The user does not exist.</exception>
    /// <exception cref="Exceptions.NotFoundException">No stock with this symbol.</exception>
    /// <exception cref="Exceptions.ConflictException">The stock is already in the portfolio.</exception>
    Task<Stock> AddStockAsync(string userId, string symbol);

    /// <exception cref="Exceptions.UnauthenticatedException">The user does not exist.</exception>
    /// <exception cref="Exceptions.NotFoundException">No such stock, or it is not in the portfolio.</exception>
    Task RemoveStockAsync(string userId, string symbol);
}
