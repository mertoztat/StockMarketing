using System.ComponentModel.DataAnnotations;

namespace StockMarkets.Models;

public class Comment
{
    public int Id { get; set; }
    public int StockId { get; set; }
    [MaxLength(280)]
    public string Content { get; set; } = string.Empty;
    [MaxLength(280)]
    public string Title { get; set; } = string.Empty;
    public Stock? Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
