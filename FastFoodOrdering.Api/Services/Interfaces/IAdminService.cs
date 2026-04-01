using FastFoodOrdering.Api.DTOs.Product;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<ProductListItemDto>> GetAllProductAsync();
    Task<ProductDetailDto?> GetProductByIdAsync(int id);
    Task<ProductDetailDto> CreateProductAsync(CreateProductRequestDto request);
    Task<ProductDetailDto?> UpdateProductAsync(int id, UpdateProductRequestDto request);
    Task<bool> DeleteProductAsync(int id);
}
