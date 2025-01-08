using Microsoft.Extensions.Configuration;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Stripe;

namespace Skinet.Infrastructure.Services
{
    public class PaymentService(IConfiguration _config, ICartService _cartService,
    IUnitOfWork _unitOfWork) : IPaymentService
    {

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            var cart = await _cartService.GetCartAsync(cartId);
            if (cart == null) return null;
            var shippingPrice = 0m;
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
                if (deliveryMethod == null) return null;
                shippingPrice = deliveryMethod.Price;
            }
            foreach (var item in cart.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entites.Product>().GetByIdAsync(item.ProductId);
                if (productItem == null) return null;
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
            var service = new PaymentIntentService();
            PaymentIntent? intent = null;
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = ["card"],
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                };
                await service.UpdateAsync(cart.PaymentIntentId, options);
            }
            await _cartService.SetCartAsync(cart);
            return cart;
        }

    }
}