using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodOrdering.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;S

        public OrdersController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // ❌ Validate thông tin
            if (string.IsNullOrEmpty(dto.FullName) ||
                string.IsNullOrEmpty(dto.Phone) ||
                string.IsNullOrEmpty(dto.Address))
            {
                return BadRequest(new { message = "Vui lòng nhập đầy đủ thông tin" });
            }

            // 🔥 Lấy giỏ hàng
            var cart = await _cartService.GetCartByUserIdAsync(int.Parse(userId));

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return BadRequest(new { message = "Giỏ hàng trống" });
            }

            try
            {
                // ✅ Tạo đơn hàng
                var order = await _orderService.CreateOrderAsync(int.Parse(userId), dto);

                // 🔥 XÓA GIỎ HÀNG 
                await _cartService.ClearCartAsync(int.Parse(userId));

                return Ok(new { message = "Đặt hàng thành công", data = order });
            }
            catch
            {
                return StatusCode(500, new { message = "Lỗi khi tạo đơn hàng" });
            }
        }
    }
}