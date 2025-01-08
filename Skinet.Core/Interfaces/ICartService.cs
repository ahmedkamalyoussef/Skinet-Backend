using Skinet.Core.Entites;

namespace Skinet.Core.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCart?> GetCartAsync(string cartId);
        Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);
        Task<bool> DeleteCartAsync(string cartId);
    }
}