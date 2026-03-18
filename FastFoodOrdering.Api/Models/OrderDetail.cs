namespace FastFoodOrdering.Api.Models;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }   // Khóa ngoại
    public int ProductId { get; set; } // Khóa ngoại
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
