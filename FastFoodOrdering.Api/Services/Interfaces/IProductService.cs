using FastFoodOrdering.Api.DTOs;
using FastFoodOrdering.Api.DTOs.Product;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductListItemDto>> GetAllProductAsync();
    Task<ProductDetailDto?> GetProductByIdAsync(int id);
}
