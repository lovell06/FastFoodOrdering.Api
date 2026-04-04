using System.Security.Claims;
using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Authorization;

public sealed class AdminAccessHandler : AuthorizationHandler<AdminAccessRequirement>
{
    private readonly ApplicationDbContext _dbContext;

    public AdminAccessHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminAccessRequirement requirement)
    {
        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return;
        }

        var isAdmin = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && user.Role == UserRole.Admin);

        if (isAdmin)
        {
            context.Succeed(requirement);
        }
    }
}
