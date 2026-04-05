using FastFoodOrdering.Api.Services.Interfaces;

namespace FastFoodOrdering.Api.Services.Implementations;

public class OrderService : IOrderService
{
    public async Task<object> CreateOrderAsync(int userId, CreateOrderDto dto)
    {
        // 👉 TODO: lưu DB
        return new
        {
            userId,
            dto.FullName,
            dto.Phone,
            dto.Address,
            dto.Note,
            createdAt = DateTime.Now
        };
    }

}
