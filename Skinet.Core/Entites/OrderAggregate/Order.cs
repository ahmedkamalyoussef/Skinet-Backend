using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skinet.Core.Entites.OrderAggregate
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public required string BuyerEmail { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = null!;
        public PaymentSummary PaymentSummary { get; set; } = null!;
        public IReadOnlyList<OrderItem> OrderItems { get; set; } = [];
        public DeliveryMethod DeliveryMethod { get; set; } = null!;
        public decimal SubTotal { get; set; }
        public OrderStatus Status { get; set; }= OrderStatus.Pending;
        public required string PaymentIntentId { get; set; }
    }
}