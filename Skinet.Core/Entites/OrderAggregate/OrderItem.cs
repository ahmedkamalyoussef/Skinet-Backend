using System;
using System.Collections.Generic;
using System.Linq;
using Skinet.Core.Entites.OrderAggregate;

namespace Skinet.Core.Entites.OrderAggregate
{
    public class OrderItem:BaseEntity
    {
       public ProductItemOrdered ItemOrdered { get; set; } =null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}