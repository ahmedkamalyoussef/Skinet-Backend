namespace Skinet.Core.Entites
{
    public class ShoppingCart
    {
        public required string Id { get; set; }
        public List<CartItem> Items { get; set; } = [];
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        public AppCoupon? Coupon { get; set; }
    }
}