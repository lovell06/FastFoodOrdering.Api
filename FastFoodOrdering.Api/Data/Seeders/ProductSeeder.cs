using FastFoodOrdering.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Data.Seeders;

public static class ProductSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Products.Any())
            return;

        context.Products.AddRange(
            new Product
            {
                Name = "Burger bò phô mai",
                Price = 50000,
                Description = "Burger bò phô mai",
                ImageUrl = "/images/products/burger-bo-pho-mai.jpg",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Gà rán truyền thống",
                Price = 60000,
                Description = "Gà chiên giòn",
                ImageUrl = "/images/products/ga-ran-truyen-thong.webp",
                StockQuantity = 100

            },
            new Product
            {
                Name = "Gà cay Hàn Quốc",
                Price = 65000,
                Description = "Gà cay",
                ImageUrl = "/images/products/ga-ran-han-quoc.webp",
                StockQuantity = 100

            },
            new Product
            {
                Name = "Khoai tây chiên",
                Price = 30000,
                Description = "Khoai chiên",
                ImageUrl = "/images/products/khoai-tay-chien.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Hotdog truyền thống",
                Price = 35000,
                Description = "Hotdog",
                ImageUrl = "/images/products/hotdog.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Pizza xúc xích",
                Price = 90000,
                Description = "Pizza",
                ImageUrl = "/images/products/pizza-xucxich.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Coca Cola",
                Price = 15000,
                Description = "Nước ngọt",
                ImageUrl = "/images/products/cocacola.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Trà sữa",
                Price = 40000,
                Description = "Trà sữa",
                ImageUrl = "/images/products/tra-sua.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Kem vani",
                Price = 20000,
                Description = "Kem",
                ImageUrl = "/images/products/kem-vani.webp",
                StockQuantity = 100             
            }
        );

        context.SaveChanges();
    }

    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Products.AnyAsync(cancellationToken))
            return;

        await context.AddRangeAsync(
            new Product
            {
                Name = "Burger bò phô mai",
                Price = 50000,
                Description = "Burger bò phô mai",
                ImageUrl = "/images/products/burger-bo-pho-mai.jpg",
                StockQuantity = 100

            },
            new Product
            {
                Name = "Gà rán truyền thống",
                Price = 60000,
                Description = "Gà chiên giòn",
                ImageUrl = "/images/products/ga-ran-truyen-thong.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Gà cay Hàn Quốc",
                Price = 65000,
                Description = "Gà cay",
                ImageUrl = "/images/products/ga-ran-han-quoc.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Khoai tây chiên",
                Price = 30000,
                Description = "Khoai chiên",
                ImageUrl = "/images/products/khoai-tay-chien.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Hotdog truyền thống",
                Price = 35000,
                Description = "Hotdog",
                ImageUrl = "/images/products/hotdog.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Pizza xúc xích",
                Price = 90000,
                Description = "Pizza",
                ImageUrl = "/images/products/pizza-xucxich.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Coca Cola",
                Price = 15000,
                Description = "Nước ngọt",
                ImageUrl = "/images/products/cocacola.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Trà sữa",
                Price = 40000,
                Description = "Trà sữa",
                ImageUrl = "/images/products/tra-sua.webp",
                StockQuantity = 100
            },
            new Product
            {
                Name = "Kem vani",
                Price = 20000,
                Description = "Kem",
                ImageUrl = "/images/products/kem-vani.webp",
                StockQuantity = 100     
            }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
