namespace StockMarkets.Models;

public class Comment    
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Stock Stock { get; set; } = new Stock();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
