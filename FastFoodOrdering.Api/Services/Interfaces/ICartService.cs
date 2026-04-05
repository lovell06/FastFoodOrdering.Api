using FastFoodOrdering.Api.DTOs.Cart;
using FastFoodOrdering.Api.Models;

namespace FastFoodOrdering.Api.Services.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddItemToCartAsync(int userId, AddToCartDto addToCartDto);

        Task<Cart> GetCartByUserIdAsync(int userId);
        Task ClearCartAsync(int userId);
    }
}
