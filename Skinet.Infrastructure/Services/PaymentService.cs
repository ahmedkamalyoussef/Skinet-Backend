using Microsoft.Extensions.Configuration;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Stripe;

namespace Skinet.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        public ICartService _cartService;
        public IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration _config, ICartService cartService,
                                IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

        }
        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
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
        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundoptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };
            var refundService = new RefundService();
            var result = await refundService.CreateAsync(refundoptions);
            return result.Status;
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