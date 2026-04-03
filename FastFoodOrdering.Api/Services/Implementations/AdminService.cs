using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Product;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class AdminService : IAdminService
{
    private const string ProductImageFolder = "Images";
    private const string ProductImageSubFolder = "products";

    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _env;

    public AdminService(ApplicationDbContext dbContext, IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
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

    private string GetUploadFolderPath()
    {
        var webRootPath = _env.WebRootPath;
        var folderPath = Path.Combine(webRootPath, ProductImageFolder, ProductImageSubFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath;
    }

    private async Task<string> SaveProductImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return string.Empty;
        }

        var uploadFolderPath = GetUploadFolderPath();
        var fileName = BuildProductImageFileName(file.FileName, uploadFolderPath);
        var relativePath = Path.Combine(ProductImageFolder, ProductImageSubFolder, fileName).Replace("\\", "/");
        var absolutePath = Path.Combine(uploadFolderPath, fileName);

        await using var stream = File.Create(absolutePath);
        await file.CopyToAsync(stream);

        return relativePath;
    }

    private static string BuildProductImageFileName(string originalFileName, string uploadFolderPath)
    {
        var extension = Path.GetExtension(originalFileName);
        var baseFileName = Path.GetFileNameWithoutExtension(originalFileName).Trim();
        if (string.IsNullOrWhiteSpace(baseFileName))
        {
            baseFileName = "product";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedBaseName = new string(baseFileName
            .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
            .ToArray())
            .Replace(' ', '-')
            .Trim('-', '.');

        if (string.IsNullOrWhiteSpace(sanitizedBaseName))
        {
            sanitizedBaseName = "product";
        }

        var fileName = $"{sanitizedBaseName}{extension}";
        if (!File.Exists(Path.Combine(uploadFolderPath, fileName)))
        {
            return fileName;
        }

        return $"{sanitizedBaseName}-{Guid.NewGuid():N}{extension}";
    }

    public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequestDto request)
    {
        var imageUrl = await SaveProductImageAsync(request.Image);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = imageUrl,
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

        var imageUrl = await SaveProductImageAsync(request.Image);
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            product.ImageUrl = imageUrl;
        }
        else if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            product.ImageUrl = request.ImageUrl;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

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
