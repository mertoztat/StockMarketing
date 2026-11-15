using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockMarkets.DTOs;
using StockMarkets.Extensions;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;
using StockMarkets.Models;

namespace StockMarkets.Controllers;

[Route("api/portfolio")]
[ApiController]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IStockRepository _stockRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioController(
        UserManager<AppUser> userManager,
        IStockRepository stockRepository,
        IPortfolioRepository portfolioRepository)
    {
        _userManager = userManager;
        _stockRepository = stockRepository;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StockDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserPortfolio()
    {
        var appUser = await GetCurrentUserAsync();
        if (appUser is null) return Unauthorized();

        var stocks = await _portfolioRepository.GetUserPortfolioAsync(appUser);
        return Ok(stocks.Select(stock => stock.ToStockDto()));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToPortfolio([FromQuery] string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Symbol is required.");

        var appUser = await GetCurrentUserAsync();
        if (appUser is null) return Unauthorized();

        var stock = await _stockRepository.GetBySymbolAsync(symbol);
        if (stock is null) return NotFound("Stock not found.");

        if (await _portfolioRepository.ExistsAsync(appUser.Id, stock.Id))
            return Conflict("Stock already exists in portfolio.");

        var portfolio = new Portfolio { AppUserId = appUser.Id, StockId = stock.Id };
        await _portfolioRepository.CreateAsync(portfolio);

        return CreatedAtAction(nameof(GetUserPortfolio), null, stock.ToStockDto());
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromPortfolio([FromQuery] string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Symbol is required.");

        var appUser = await GetCurrentUserAsync();
        if (appUser is null) return Unauthorized();

        var stock = await _stockRepository.GetBySymbolAsync(symbol);
        if (stock is null) return NotFound("Stock not found.");

        var deleted = await _portfolioRepository.DeleteAsync(appUser.Id, stock.Id);
        if (deleted is null) return NotFound("Stock is not in your portfolio.");

        return NoContent();
    }

    private async Task<AppUser?> GetCurrentUserAsync()
    {
        var username = User.GetUsername();
        return string.IsNullOrEmpty(username)
            ? null
            : await _userManager.FindByNameAsync(username);
    }
}
