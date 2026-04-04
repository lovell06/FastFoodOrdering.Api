namespace FastFoodOrdering.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int StockQuantity { get; set; }

    private bool _isAvailable;
    public bool IsAvailable
    {
        get => _isAvailable;
        set => _isAvailable = value;
    }

    // 1 Product nằm trong nhiều OrderDetails
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
