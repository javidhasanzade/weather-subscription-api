using Microsoft.EntityFrameworkCore;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasIndex(s => s.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}