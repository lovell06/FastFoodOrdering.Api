using FastFoodOrdering.Api.DTOs.Cart;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodOrdering.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
            }
            
            var cart = await _cartService.GetCartByUserIdAsync(int.Parse(userId));

            if(cart == null)
            {
                return Ok(new { message = "Giỏ hàng của bạn đang trống." });
            }
            return Ok(cart);
        }

        // POST: /api/cart/item
        [HttpPost("item")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            if (userId == null)
            {
                return Unauthorized("Không thế xác mình người dùng");
            }
            try
            {
                var res = await _cartService.AddItemToCartAsync(int.Parse(userId), addToCartDto);
                if(res)
                {
                    return Ok(new { message = "Sản phẩm đã được thêm vào giỏ hàng." });
                }
                return BadRequest(new { message = "Không thể thêm sản phẩm vào giỏ hàng." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng." });
            }
        }

    }
}
