namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IOrderService
{
    Task<object> CreateOrderAsync(int userId, CreateOrderDto dto);
}
