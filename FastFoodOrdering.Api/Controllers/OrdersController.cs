using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Order;
using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // 🔥 API HỦY ĐƠN HÀNG
    [HttpPut("{orderId}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(int orderId, [FromServices] ApplicationDbContext dbContext)
    {
        // 1. Nhận diện User từ Token
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized(new { message = "Vui lòng đăng nhập!" });
        int userId = int.Parse(userIdString);

        // 2. Tìm đơn hàng
        var order = await dbContext.Orders.FindAsync(orderId);

        // AC1 & AC4: Kiểm tra quyền sở hữu
        if (order == null || order.UserId != userId)
        {
            return BadRequest(new { message = "Đơn hàng không tồn tại hoặc bạn không có quyền hủy!" });
        }

        // AC1 & AC4: Chỉ cho phép hủy khi đơn hàng đang ở trạng thái Pending (0)
        if (order.Status != OrderStatus.Pending)
        {
            return BadRequest(new { message = "Từ chối xử lý: Đơn hàng đã được xác nhận hoặc đã thanh toán, không thể hủy!" });
        }

        // AC3: Cập nhật trạng thái sang Cancelled (3)
        order.Status = OrderStatus.Cancelled;

        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Hủy đơn hàng thành công!" });
    }

    // 🔥 API LẤY LỊCH SỬ ĐƠN HÀNG CỦA USER
    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetOrderHistory([FromServices] ApplicationDbContext dbContext)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        int userId = int.Parse(userIdString);

        var orders = await dbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate) // Sắp xếp đơn mới nhất lên đầu
            .Select(o => new {
                o.Id,
                o.OrderDate,
                o.TotalAmount,
                o.Status
            }).ToListAsync();

        return Ok(orders);
    }
}