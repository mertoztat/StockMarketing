using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StockMarkets.Models;

namespace StockMarkets.Data;

public class ApplicationDBContext : IdentityDbContext<AppUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions) : base(dbContextOptions)
    {

    }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<Portfolio> Portfolios { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Portfolio>()
            .HasKey(p => new { p.AppUserId, p.StockId });

        builder.Entity<Portfolio>()
            .HasOne(u => u.AppUser)
            .WithMany(p => p.Portfolios)
            .HasForeignKey(p => p.AppUserId);

        builder.Entity<Portfolio>()
            .HasOne(s => s.Stock)
            .WithMany(p => p.Portfolios)
            .HasForeignKey(p => p.StockId);

        var roles = new List<IdentityRole>
        {
            new() { Name = "Admin", NormalizedName = "ADMIN" }, new() { Name = "User", NormalizedName = "USER" }
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}
