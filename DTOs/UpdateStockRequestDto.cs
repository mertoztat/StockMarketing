using System.ComponentModel.DataAnnotations;

namespace StockMarkets.DTOs;

public class UpdateStockRequestDto
{
    [Required]
    [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(100, ErrorMessage = "CompanyName cannot be over 100 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Range(0.01, 1000000000)]
    public decimal Purchase { get; set; }

    [Range(0, 100)]
    public decimal LastDividend { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "Industry cannot be over 50 characters")]
    public string Industry { get; set; } = string.Empty;

    [Range(1, 5000000000000)]
    public long MarketCap { get; set; }
}
