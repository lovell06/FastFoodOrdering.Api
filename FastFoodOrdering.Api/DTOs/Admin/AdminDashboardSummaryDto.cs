namespace FastFoodOrdering.Api.DTOs.Admin;

public class AdminDashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public int AvailableProducts { get; set; }
    public int UnavailableProducts { get; set; }
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int PaidOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
