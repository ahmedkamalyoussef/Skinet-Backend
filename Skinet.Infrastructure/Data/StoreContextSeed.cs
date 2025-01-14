using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Skinet.Core.Entites;
using System.Reflection;
using System.Text.Json;

namespace Skinet.Infrastructure.Data;

public class StoreContextSeed
{
    private static IConfiguration _config;
    public StoreContextSeed(IConfiguration config)
    {
        _config = config;
    }
    public static async Task SeedAsync(StoreContext context, UserManager<AppUser> userManager)
    {

        if (!userManager.Users.Any(u => u.Email == "admin@test.com"))
        {
            var user = new AppUser
            {
                Email = "admin@test.com",
                UserName = "admin@test.com"
            };
            await userManager.CreateAsync(user, _config["AdminPassword"]);
            await userManager.AddToRoleAsync(user, "Admin");
        }

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products == null) return;
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        if (!context.DeliveryMethods.Any())
        {
            var dmData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);
            if (methods == null) return;
            context.DeliveryMethods.AddRange(methods);
            await context.SaveChangesAsync();
        }
    }
}