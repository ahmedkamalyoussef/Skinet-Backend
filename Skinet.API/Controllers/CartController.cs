using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;

namespace Skinet.API.Controllers
{
    public class CartController(ICartService cartService) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCartById(string id)
        {
            var cart = await cartService.GetCartAsync(id);
            return Ok(cart ?? new ShoppingCart { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(ShoppingCart cart)
        {
            var updateCart = await cartService.SetCartAsync(cart);
            return updateCart is null ? BadRequest("problem updating cart") : Ok(updateCart);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart(string id)
        {
            return await cartService.DeleteCartAsync(id) ? Ok(new { message = "Cart deleted successfully" }) : BadRequest(new { message ="problem deleting cart"});
        }
    }
}