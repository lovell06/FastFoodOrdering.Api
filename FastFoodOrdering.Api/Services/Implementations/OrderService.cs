using FastFoodOrdering.Api.DTOs.Order;
using FastFoodOrdering.Api.Services.Interfaces;
using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool IsSuccess, string Message, int? OrderId)> CreateOrderAsync(int userId, CreateOrderRequestDto dto)
    {
        // 1. Lấy Giỏ hàng và Chi tiết các món (Kèm theo thông tin Product để lấy giá gốc)
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        // AC5: Nếu giỏ hàng trống, từ chối
        if (cart == null || !cart.Items.Any())
        {
            return (false, "Giỏ hàng của bạn đang trống. Không thể đặt hàng!", null);
        }

        // 2. Tính tổng tiền (AC1, Subtask 1)
        decimal totalAmount = cart.Items.Sum(item => item.Quantity * item.Product.Price);

        // 3. Khởi tạo Đơn hàng (Order)
        var newOrder = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = (PaymentStatus)0, // Giả định 0 là Pending chưa thanh toán
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone.Trim(),
            Address = dto.Address.Trim(),
            Note = dto.Note?.Trim()
        };

        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync(); // Cần save trước để EF Core tự sinh ra Order.Id

        // 4. Tạo chi tiết đơn hàng (OrderDetails)
        var orderDetails = cart.Items.Select(item => new OrderDetail
        {
            OrderId = newOrder.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.Product.Price, // Chốt giá tại thời điểm mua
            SubTotal = item.Quantity * item.Product.Price
        }).ToList();

        _dbContext.OrderDetails.AddRange(orderDetails);

        // 5. Xóa các món trong giỏ hàng sau khi đã chuyển thành đơn hàng thành công (AC4)
        _dbContext.RemoveRange(cart.Items);

        // Chốt hạ toàn bộ xuống DB
        await _dbContext.SaveChangesAsync();

        return (true, "Đặt hàng thành công!", newOrder.Id);
    }
}
