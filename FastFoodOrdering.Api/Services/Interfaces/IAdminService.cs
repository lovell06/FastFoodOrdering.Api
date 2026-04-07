using FastFoodOrdering.Api.DTOs.Admin;
using FastFoodOrdering.Api.DTOs.Order;
using FastFoodOrdering.Api.DTOs.Product;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IAdminService
{
    Task<AdminBootstrapDto> GetBootstrapAsync();
    Task<IEnumerable<AdminProductListItemDto>> GetProductsAsync();
    Task<IEnumerable<AdminOrderListItemDto>> GetOrdersAsync();
    Task<IEnumerable<AdminUserListItemDto>> GetUsersAsync();
    Task<AdminProductDetailDto?> GetProductByIdAsync(int id);
    Task<AdminProductDetailDto> CreateProductAsync(CreateProductRequestDto request);
    Task<AdminProductDetailDto?> UpdateProductAsync(int id, UpdateProductRequestDto request);
    Task<AdminProductDetailDto?> UpdateProductAvailabilityAsync(int id, UpdateProductAvailabilityRequestDto request);
    Task<bool> DeleteProductAsync(int id);
}
