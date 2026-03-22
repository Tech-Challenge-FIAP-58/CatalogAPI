using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class CartController(ICartService service, ILogger<CartController> logger) : StandardController
    {
        [Authorize]
        [HttpGet("GetCartByUserId/{userId:int}")]
        public Task<IActionResult> GetByUserId(int userId)
        {
            logger.LogInformation("GET - Get active cart by user: {UserId}", userId);
            return TryMethodAsync(() => service.GetByUserId(userId), logger);
        }

        [Authorize]
        [HttpPost("AddItemToCart")]
        public Task<IActionResult> AddItem([FromBody] CartAddItemDto dto)
        {
            logger.LogInformation("POST - Add item to cart for user: {UserId}", dto.UserId);
            return TryMethodAsync(() => service.AddItem(dto), logger);
        }

        [Authorize]
        [HttpDelete("RemoveItemFromCart")]
        public Task<IActionResult> RemoveItem([FromBody] CartRemoveItemDto dto)
        {
            logger.LogInformation("DELETE - Remove item from cart for user: {UserId}", dto.UserId);
            return TryMethodAsync(() => service.RemoveItem(dto), logger);
        }

        [Authorize]
        [HttpDelete("ClearCart/{userId:int}")]
        public Task<IActionResult> Clear(int userId)
        {
            logger.LogInformation("DELETE - Clear cart for user: {UserId}", userId);
            return TryMethodAsync(() => service.Clear(userId), logger);
        }

        [Authorize]
        [HttpPost("CheckoutCart")]
        public Task<IActionResult> Checkout([FromBody] CheckoutCartDto dto)
        {
            logger.LogInformation("POST - Checkout cart for user: {UserId}", dto.ClientId);
            return TryMethodAsync(() => service.Checkout(dto), logger);
        }
    }
}
