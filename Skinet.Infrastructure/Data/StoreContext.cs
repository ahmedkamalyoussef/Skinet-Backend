using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skinet.Core.Entites;
using Skinet.Core.Entites.OrderAggregate;
using Skinet.Infrastructure.Configurations;

namespace Skinet.Infrastructure.Data;

public class StoreContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderItemConfiguration).Assembly);
    }
}