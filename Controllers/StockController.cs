using Microsoft.EntityFrameworkCore;
using StockMarkets.Mappers;
using Microsoft.AspNetCore.Mvc;
using StockMarkets.Data;
using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase
{
   private readonly ApplicationDBContext _context;

   public StockController(ApplicationDBContext context)
   {
      _context = context;
   }

   [HttpGet]
   public async Task<IActionResult> GetAllStocks()
   {
      var stocks = await _context.Stocks.ToListAsync();
      var stockDtos = stocks.Select(stock => stock.ToStockDto());
      return Ok(stockDtos);
   }

   [HttpGet("{id}")]
   public async Task<IActionResult> GetStockById([FromRoute] int id)
   {
      var stock = await _context.Stocks.FindAsync(id);
      if (stock == null)
      {
         return NotFound();
      }

      return Ok(stock.ToStockDto());
   }

   [HttpPost]
   public async Task<IActionResult> CreateStock([FromBody] CreateStockRequestDto stockDto)
   {
      var stockModel = stockDto.ToStockFromCreateDto();
      _context.Stocks.Add(stockModel);
      await _context.SaveChangesAsync();
      return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ToStockDto());
   }

   [HttpPut]
   [Route("{id}")]
   public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
   {
      var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
      if (stockModel == null)
      {
         return NotFound();
      }

      stockModel.UpdateFromDto(updateDto);
      await _context.SaveChangesAsync();
      return NoContent();
   }

   [HttpDelete]
   [Route("{id}")]
   public async Task<IActionResult> DeleteStock([FromRoute] int id)
   {
      var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
      if (stockModel == null)
      {
         return NotFound();
      }

      _context.Stocks.Remove(stockModel);
      await _context.SaveChangesAsync();
      return NoContent();
   }
}