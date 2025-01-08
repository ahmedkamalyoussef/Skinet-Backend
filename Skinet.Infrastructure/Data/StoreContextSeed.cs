using Skinet.Core.Entites;
using System.Text.Json;

namespace Skinet.Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync("../Skinet.Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products == null) return;
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        if (!context.DeliveryMethods.Any())
        {
            var dmData = await File.ReadAllTextAsync("../Skinet.Infrastructure/Data/SeedData/delivery.json");
            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);
            if (methods == null) return;
            context.DeliveryMethods.AddRange(methods);
            await context.SaveChangesAsync();
        }
    }
}