using FastFoodOrdering.Api.DTOs.Order;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IOrderService
{
    Task<(bool IsSuccess, string Message, int? OrderId)> CreateOrderAsync(int userId, CreateOrderRequestDto dto);
}
