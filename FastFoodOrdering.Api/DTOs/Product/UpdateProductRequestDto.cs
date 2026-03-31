namespace FastFoodOrdering.Api.DTOs.Product;

public class UpdateProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
