namespace FastFoodOrdering.Api.DTOs.Admin;

public class AdminProductListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
}
