using Skinet.Core.Entites.OrderAggregate;

namespace Skinet.Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string email) : base(o => o.BuyerEmail == email)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            SetOrderByDescending(x => x.OrderDate);
        }
        public OrderSpecification(int id, string email) : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            SetOrderByDescending(x => x.OrderDate);
        }
        public OrderSpecification(string paymentIntentId, bool isPaymentIntent) : base(o => o.PaymentIntentId == paymentIntentId)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            SetOrderByDescending(x => x.OrderDate);
        }
    }
}