using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skinet.Core.Entites;

namespace Skinet.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
    }
}