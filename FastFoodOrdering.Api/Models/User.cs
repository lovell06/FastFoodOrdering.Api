using FastFoodOrdering.Api.Enums;
namespace FastFoodOrdering.Api.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // 1 User có nhiều Orders
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
