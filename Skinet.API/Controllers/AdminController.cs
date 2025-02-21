using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skinet.API.Extensions;
using Skinet.Core.Entites.OrderAggregate;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;

namespace Skinet.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork _unitOfWork, IPaymentService _paymentService) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(OrderSpecParams orderSpecParams)
        {
            var spec = new OrderSpecification(orderSpecParams);
            return await CreatePagedResult(_unitOfWork.Repository<Order>(), spec, orderSpecParams.PageIndex, orderSpecParams.PageSize, o => o.ToDto());
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecification(spec);
            return order != null ? Ok(order.ToDto()) : BadRequest("not fount");
        }
        [HttpPost("orders/refund/{id}")]
        public async Task<IActionResult> RefundOrder(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecification(spec);
            if (order == null) return BadRequest("order not found");
            if (order.Status == OrderStatus.Pending) return BadRequest("payment not received for this order");
            var result = await _paymentService.RefundPayment(order.PaymentIntentId);
            if (result == "succeeded")
            {
                order.Status = OrderStatus.Refunded;
                await _unitOfWork.Complete();
                return Ok(order.ToDto());
            }
            return BadRequest("problem refunding order");
        }
    }
}
