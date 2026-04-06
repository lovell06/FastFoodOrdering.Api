using FastFoodOrdering.Api.DTOs.Order;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodOrdering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Bắt buộc phải đăng nhập mới được đặt hàng
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequestDto request)
    {
        // 1. Lấy UserId từ Token đang đăng nhập
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Vui lòng đăng nhập để thực hiện đặt hàng." });
        }

        // 2. Validate DTO (AC2)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 3. Gọi Service xử lý
        var result = await _orderService.CreateOrderAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message, orderId = result.OrderId });
    }
}