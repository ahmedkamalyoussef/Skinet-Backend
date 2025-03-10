using Skinet.Core.Interfaces;

namespace Skinet.Core.Entites.OrderAggregate
{
    public class Order : BaseEntity, IDtoConvertable
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public required string BuyerEmail { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = null!;
        public PaymentSummary PaymentSummary { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = [];
        public DeliveryMethod DeliveryMethod { get; set; } = null!;
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required string PaymentIntentId { get; set; }
        public decimal GetTotal() => SubTotal - Discount + DeliveryMethod.Price;
    }
}