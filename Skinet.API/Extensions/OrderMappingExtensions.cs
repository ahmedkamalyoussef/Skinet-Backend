using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skinet.API.DTOs;
using Skinet.Core.Entites.OrderAggregate;

namespace Skinet.API.Extensions
{
    public static class OrderMappingExtensions
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                ShippingAddress = order.ShippingAddress,
                OrderDate = order.OrderDate,
                PaymentSummary = order.PaymentSummary,
                DeliveryMethod = order.DeliveryMethod.Description,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems = order.OrderItems.Select(x => x.ToDto()).ToList(),
                SubTotal = order.SubTotal,
                Status = order.Status.ToString(),
                PaymentIntentId=order.PaymentIntentId,
                Total = order.GetTotal()
            };
        }

        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                ProductId = orderItem.ItemOrdered.ProductId,
                ProductName = orderItem.ItemOrdered.ProductName,
                PictureUrl = orderItem.ItemOrdered.PictureUrl,
                Price=orderItem.Price,
                Quantity = orderItem.Quantity
            };
        }
    }
}