using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Skinet.API.Extensions;
using Skinet.API.SignalR;
using Skinet.Core.Entites;
using Skinet.Core.Entites.OrderAggregate;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;
using Stripe;

namespace Skinet.API.Controllers
{
    public class PaymentsController(IPaymentService _paymentService, IUnitOfWork _unitOfWork,
    ILogger<PaymentsController> _logger, IHubContext<NotificationHub> hubContext, IConfiguration _config) : BaseApiController
    {
        private readonly string _whSecret = _config["Stripe:WhSecret"]!;

        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await _paymentService.CreateOrUpdatePaymentIntent(cartId);
            if (cart == null) return BadRequest("Problem with your cart");
            return Ok(cart);
        }
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
            return Ok(deliveryMethods);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = ConstructStripeEvent(json);
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("invalid event data");
                }

                await HandlePaymentIntentSuccess(intent);
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "WebHook error");
                return StatusCode(StatusCodes.Status500InternalServerError, "WebHook error");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured");
            }
        }

        private async Task HandlePaymentIntentSuccess(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                var spec = new OrderSpecification(intent.Id, true);
                var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecification(spec)
                    ?? throw new Exception("order not found");

                if ((long)order.GetTotal() * 100 != intent.Amount)
                    order.Status = OrderStatus.PaymentMismatch;
                else
                    order.Status = OrderStatus.PaymentReceived;

                await _unitOfWork.Complete();

                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
                if (!string.IsNullOrEmpty(connectionId))
                    await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret, throwOnApiVersionMismatch: false);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "failed to construct stripe event");
                throw new StripeException("invalid Signature");
            }
        }
    }
}