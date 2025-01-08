using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skinet.API.DTOs;
using Skinet.API.Extensions;
using Skinet.Core.Entites;
using Skinet.Core.Entites.OrderAggregate;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;

namespace Skinet.API.Controllers
{
    [Authorize]
    public class OrdersController(ICartService _cartService, IUnitOfWork _unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();
            var cart = await _cartService.GetCartAsync(orderDto.CartId);
            if (cart == null) return BadRequest("Problem with your cart");
            if (cart.PaymentIntentId == null) return BadRequest("Problem with your payment");
            var items = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (productItem == null) return BadRequest("Problem with your cart");
                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    ProductName = productItem.Name,
                    PictureUrl = productItem.ImageUrl
                };
                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);
            }
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod == null) return BadRequest("Problem with your cart");
            var subtotal = items.Sum(item => item.Price * item.Quantity);
            var order = new Order
            {
                OrderItems = items,
                DeliveryMethod = deliveryMethod,
                ShippingAddress = orderDto.ShippingAddress,
                SubTotal = subtotal,
                PaymentSummary = orderDto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                BuyerEmail = email
            };
            _unitOfWork.Repository<Order>().Add(order);
            if (await _unitOfWork.Complete()) return order;
            return BadRequest("Problem with your order");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders()
        {
            var spec = new OrderSpecification(User.GetEmail());
            var orders = await _unitOfWork.Repository<Order>().ListAsync(spec);
            var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id, User.GetEmail());
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecification(spec);
            if (order == null) return NotFound();
            return Ok(order.ToDto());
        }
    }

}
