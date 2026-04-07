using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Admin;
using FastFoodOrdering.Api.DTOs.Order;
using FastFoodOrdering.Api.DTOs.Product;
using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class AdminService : IAdminService
{
    private const string ProductImageFolder = "images";
    private const string ProductImageSubFolder = "products";

    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _env;

    public AdminService(ApplicationDbContext dbContext, IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
    }

    public async Task<AdminBootstrapDto> GetBootstrapAsync()
    {
        var totalProducts = await _dbContext.Products.AsNoTracking().CountAsync();
        var availableProducts = await _dbContext.Products.AsNoTracking().CountAsync(p => p.IsAvailable);
        var totalUsers = await _dbContext.Users.AsNoTracking().CountAsync();
        var totalAdmins = await _dbContext.Users.AsNoTracking().CountAsync(u => u.Role == UserRole.Admin);
        var totalOrders = await _dbContext.Orders.AsNoTracking().CountAsync();
        var pendingOrders = await _dbContext.Orders.AsNoTracking().CountAsync(o => o.Status == OrderStatus.Pending);
        var paidOrders = await _dbContext.Orders.AsNoTracking().CountAsync(o => o.PaymentStatus == PaymentStatus.Paid);
        var totalRevenue = await _dbContext.Orders.AsNoTracking()
            .Where(o => o.PaymentStatus == PaymentStatus.Paid)
            .Select(o => (decimal?)o.TotalAmount)
            .SumAsync() ?? 0m;

        return new AdminBootstrapDto
        {
            Summary = new AdminDashboardSummaryDto
            {
                TotalProducts = totalProducts,
                AvailableProducts = availableProducts,
                UnavailableProducts = totalProducts - availableProducts,
                TotalUsers = totalUsers,
                TotalCustomers = totalUsers - totalAdmins,
                TotalAdmins = totalAdmins,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                PaidOrders = paidOrders,
                TotalRevenue = totalRevenue
            },
            RecentOrders = await GetOrderListAsync(5),
            LatestUsers = await GetUserListAsync(5),
            ProductsNeedingAttention = await _dbContext.Products.AsNoTracking()
                .OrderBy(p => p.IsAvailable ? 1 : 0)
                .ThenBy(p => p.StockQuantity)
                .ThenBy(p => p.Name)
                .Take(5)
                .Select(p => new AdminProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    IsAvailable = p.IsAvailable
                })
                .ToListAsync()
        };
    }

    public async Task<IEnumerable<AdminProductListItemDto>> GetProductsAsync()
    {
        return await _dbContext.Products.AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Select(p => new AdminProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AdminOrderListItemDto>> GetOrdersAsync()
    {
        return await GetOrderListAsync();
    }

    public async Task<IEnumerable<AdminUserListItemDto>> GetUsersAsync()
    {
        return await GetUserListAsync();
    }

    public async Task<AdminProductDetailDto?> GetProductByIdAsync(int id)
    {
        return await _dbContext.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new AdminProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable
            })
            .FirstOrDefaultAsync();
    }

    private string GetUploadFolderPath()
    {
        var webRootPath = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
        }

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
        var relativePath = BuildProductImageRelativePath(fileName);
        var absolutePath = Path.Combine(uploadFolderPath, fileName);

        await using var stream = File.Create(absolutePath);
        await file.CopyToAsync(stream);

        return relativePath;
    }

    private static string BuildProductImageRelativePath(string fileName)
    {
        return $"/{ProductImageFolder}/{ProductImageSubFolder}/{fileName}";
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

    public async Task<AdminProductDetailDto> CreateProductAsync(CreateProductRequestDto request)
    {
        var imageUrl = await SaveProductImageAsync(request.Image);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = imageUrl,
            StockQuantity = request.StockQuantity,
            IsAvailable = request.IsAvailable
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return ToAdminProductDetailDto(product);
    }

    public async Task<AdminProductDetailDto?> UpdateProductAsync(int id, UpdateProductRequestDto request)
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

        if (request.StockQuantity.HasValue)
        {
            product.StockQuantity = request.StockQuantity.Value;
        }

        if (request.IsAvailable.HasValue)
        {
            product.IsAvailable = request.IsAvailable.Value;
        }

        await _dbContext.SaveChangesAsync();

        return ToAdminProductDetailDto(product);
    }

    public async Task<AdminProductDetailDto?> UpdateProductAvailabilityAsync(int id, UpdateProductAvailabilityRequestDto request)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }

        product.IsAvailable = request.IsAvailable;
        await _dbContext.SaveChangesAsync();

        return ToAdminProductDetailDto(product);
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

    private async Task<List<AdminOrderListItemDto>> GetOrderListAsync(int? take = null)
    {
        IQueryable<Order> query = _dbContext.Orders.AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id);

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        var orders = await query
            .Select(o => new
            {
                o.Id,
                o.UserId,
                CustomerName = o.User.FullName,
                CustomerEmail = o.User.Email,
                o.OrderDate,
                o.TotalAmount,
                o.Status,
                o.PaymentMethod,
                o.PaymentStatus,
                TotalItems = o.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0
            })
            .ToListAsync();

        return orders
            .Select(o => new AdminOrderListItemDto
            {
                Id = o.Id,
                UserId = o.UserId,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                TotalItems = o.TotalItems
            })
            .ToList();
    }

    private async Task<List<AdminUserListItemDto>> GetUserListAsync(int? take = null)
    {
        IQueryable<User> query = _dbContext.Users.AsNoTracking()
            .OrderByDescending(u => u.Id);

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        var users = await query
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Phone,
                u.Role,
                TotalOrders = u.Orders.Count()
            })
            .ToListAsync();

        return users
            .Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                TotalOrders = u.TotalOrders
            })
            .ToList();
    }

    private static AdminProductDetailDto ToAdminProductDetailDto(Product product)
    {
        return new AdminProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable
        };
    }
}
