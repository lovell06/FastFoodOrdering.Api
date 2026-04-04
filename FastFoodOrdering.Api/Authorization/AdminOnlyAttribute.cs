using Microsoft.AspNetCore.Authorization;

namespace FastFoodOrdering.Api.Authorization;

public sealed class AdminOnlyAttribute : AuthorizeAttribute
{
    public AdminOnlyAttribute()
    {
        Policy = AuthorizationPolicies.AdminOnly;
    }
}
