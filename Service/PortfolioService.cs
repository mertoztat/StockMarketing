using Microsoft.AspNetCore.Identity;
using StockMarkets.Exceptions;
using StockMarkets.Interfaces;
using StockMarkets.Models;

namespace StockMarkets.Service;

public class PortfolioService : IPortfolioService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IStockRepository _stockRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioService(
        UserManager<AppUser> userManager,
        IStockRepository stockRepository,
        IPortfolioRepository portfolioRepository)
    {
        _userManager = userManager;
        _stockRepository = stockRepository;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<List<Stock>> GetUserPortfolioAsync(string userId)
    {
        var user = await GetUserOrThrowAsync(userId);
        return await _portfolioRepository.GetUserPortfolioAsync(user);
    }

    public async Task<Stock> AddStockAsync(string userId, string symbol)
    {
        var user = await GetUserOrThrowAsync(userId);

        var stock = await _stockRepository.GetBySymbolAsync(symbol)
                    ?? throw new NotFoundException("Stock not found.");

        if (await _portfolioRepository.ExistsAsync(user.Id, stock.Id))
            throw new ConflictException(ErrorCodes.AlreadyInPortfolio, "Stock already exists in portfolio.");

        await _portfolioRepository.CreateAsync(new Portfolio { AppUserId = user.Id, StockId = stock.Id });
        return stock;
    }

    public async Task RemoveStockAsync(string userId, string symbol)
    {
        var user = await GetUserOrThrowAsync(userId);

        var stock = await _stockRepository.GetBySymbolAsync(symbol)
                    ?? throw new NotFoundException("Stock not found.");

        var deleted = await _portfolioRepository.DeleteAsync(user.Id, stock.Id);
        if (deleted is null) throw new NotFoundException("Stock is not in your portfolio.");
    }

    private async Task<AppUser> GetUserOrThrowAsync(string userId)
    {
        var user = string.IsNullOrEmpty(userId) ? null : await _userManager.FindByIdAsync(userId);
        return user ?? throw new UnauthenticatedException("User not found.");
    }
}
