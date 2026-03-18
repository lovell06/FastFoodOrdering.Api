using FastFoodOrdering.Api.Enums;
namespace FastFoodOrdering.Api.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // Khóa ngoại
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    // 1 Order có nhiều OrderDetails
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
