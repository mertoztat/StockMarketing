using Microsoft.AspNetCore.Mvc;
using StockMarkets.DTOs;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;

namespace StockMarkets.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase
{
   private readonly IStockRepository _stockRepository;

   public StockController(IStockRepository stockRepository)
   {
      _stockRepository = stockRepository;
   }

   [HttpGet]
   public async Task<IActionResult> GetAllStocks()
   {
      var stocks = await _stockRepository.GetAllAsync();
      var stockDtos = stocks.Select(stock => stock.ToStockDto());
      return Ok(stockDtos);
   }

   [HttpGet("{id}")]
   public async Task<IActionResult> GetStockById([FromRoute] int id)
   {
      var stock = await _stockRepository.GetByIdAsync(id);
      
      if (stock == null)
      {
         return NotFound();
      }

      return Ok(stock.ToStockDto());
   }

   [HttpPost]
   public async Task<IActionResult> CreateStock([FromBody] CreateStockRequestDto stockDto)
   {
      if (await _stockRepository.SymbolExistsAsync(stockDto.Symbol))
      {
         return Conflict("A stock with this symbol already exists.");
      }

      var stockModel = stockDto.ToStockFromCreateDto();
      await _stockRepository.CreateAsync(stockModel);
      return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ToStockDto());
   }

   [HttpPut]
   [Route("{id}")]
   public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
   {
      if (await _stockRepository.SymbolExistsAsync(updateDto.Symbol, id))
      {
         return Conflict("A stock with this symbol already exists.");
      }

      var stockModel = await _stockRepository.UpdateAsync(id, updateDto);
      if (stockModel == null)
      {
         return NotFound();
      }

      return Ok(stockModel.ToStockDto());
   }

   [HttpDelete]
   [Route("{id}")]
   public async Task<IActionResult> DeleteStock([FromRoute] int id)
   {
      var stockModel = await _stockRepository.DeleteAsync(id);
      if (stockModel == null)
      {
         return NotFound();
      }

      return NoContent();
   }
}