using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Product;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;
    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<ProductListItemDto>> GetAllProductAsync()
    {
        var products = await _dbContext.Products
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
        
        return products;
    }
    public async Task<ProductDetailDto> GetProductByIdAsync(int id)
    {   
        var product = await _dbContext.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).FirstOrDefaultAsync();
        
        return product;
    }
}