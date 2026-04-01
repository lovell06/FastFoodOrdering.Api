using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Product;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _dbContext;

    public AdminService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductListItemDto>> GetAllProductAsync()
    {
        return await _dbContext.Products
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
    {
        return await _dbContext.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequestDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = true
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };
    }

    public async Task<ProductDetailDto?> UpdateProductAsync(int id, UpdateProductRequestDto request)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;

        await _dbContext.SaveChangesAsync();

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
