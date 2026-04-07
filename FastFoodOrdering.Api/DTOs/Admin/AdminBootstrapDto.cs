using FastFoodOrdering.Api.DTOs.Order;

namespace FastFoodOrdering.Api.DTOs.Admin;

public class AdminBootstrapDto
{
    public AdminDashboardSummaryDto Summary { get; set; } = new();
    public IReadOnlyCollection<AdminOrderListItemDto> RecentOrders { get; set; } = Array.Empty<AdminOrderListItemDto>();
    public IReadOnlyCollection<AdminUserListItemDto> LatestUsers { get; set; } = Array.Empty<AdminUserListItemDto>();
    public IReadOnlyCollection<AdminProductListItemDto> ProductsNeedingAttention { get; set; } = Array.Empty<AdminProductListItemDto>();
}
