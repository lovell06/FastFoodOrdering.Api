using Microsoft.AspNetCore.Http;

namespace FastFoodOrdering.Api.DTOs.Product;

public class CreateProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}
