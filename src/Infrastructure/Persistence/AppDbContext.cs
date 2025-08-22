using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Order> Orders { get; init; }
    // Config retry for order with internal system.
    // public DbSet<OrderRetry> OrderRetries { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configuration từ Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}