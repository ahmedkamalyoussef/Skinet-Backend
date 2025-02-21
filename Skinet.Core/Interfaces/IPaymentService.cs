using Skinet.Core.Entites;

namespace Skinet.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
        Task<string> RefundPayment(string paymentIntentId);
    }
}