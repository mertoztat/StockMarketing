using Microsoft.EntityFrameworkCore;
using StockMarkets.Models;

namespace StockMarkets.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions dbContextOptions): base(dbContextOptions)
    {
        
    }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Comment> Comments { get; set; }
}