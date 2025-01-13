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
            var cart = await _cartService.GetCartAsync(cartId)
                ?? throw new Exception("Cart unavailable");
            var shippingPrice = await GetShippingPriceAsync(cart) ?? 0;
            await ValidateCartItemsInCartAsync(cart);
            var subtotal = CalculateSubtotal(cart);
            if (cart.Coupon != null)
            {
                subtotal = await ApplyDiscountAsync(cart.Coupon, subtotal);
            }
            var total = subtotal + shippingPrice;
            await CreateUpdatePaymentIntentAsync(cart, total);
            await _cartService.SetCartAsync(cart);
            return cart;
        }
        private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total)
        {
            var service = new PaymentIntentService();
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = total,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                var intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = total
                };
                await service.UpdateAsync(cart.PaymentIntentId, options);
            }
        }
        private async Task<long> ApplyDiscountAsync(AppCoupon appCoupon, long amount)
        {
            var couponService = new Stripe.CouponService();
            var coupon = await couponService.GetAsync(appCoupon.CouponId);
            if (coupon.AmountOff.HasValue)
            {
                amount -= (long)coupon.AmountOff * 100;
            }
            if (coupon.PercentOff.HasValue)
            {
                var discount = amount * (coupon.PercentOff.Value / 100);
                amount -= (long)discount;
            }
            return amount;
        }
        private long CalculateSubtotal(ShoppingCart cart)
        {
            var itemTotal = cart.Items.Sum(x => x.Quantity * x.Price * 100);
            return (long)itemTotal;
        }
        private async Task ValidateCartItemsInCartAsync(ShoppingCart cart)
        {
            foreach (var item in cart.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entites.Product>()
                    .GetByIdAsync(item.ProductId)
                    ?? throw new Exception("Problem getting product in cart");
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
        }
        private async Task<long?> GetShippingPriceAsync(ShoppingCart cart)
        {
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync((int)cart.DeliveryMethodId)
                        ?? throw new Exception("Problem with delivery method");
                return (long)deliveryMethod.Price * 100;
            }
            return null;
        }

    }
}

/*
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
        */