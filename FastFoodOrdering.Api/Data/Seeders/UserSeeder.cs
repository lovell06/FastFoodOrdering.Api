using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Data.Seeders;

public static class UserSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Users.Any())
            return;

        var passwordHasher = new PasswordHasher<User>();

        var user = new User
        {
            Email = "admin012@gmail.com",
            FullName = "admin",
            Phone = "0999999999",
            Role = UserRole.Admin
        };
        user.Password = passwordHasher.HashPassword(user, "Adminpassword123@");

        context.Users.AddRange(
            user
        );

        context.SaveChanges();
    }

    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        await context.Users.AddRangeAsync(
            new User
            {
                Email = "admin012@gmail.com",
                FullName = "admin",
                Password = "adminpassword123",
                Phone = "0999999999",
                Role = UserRole.Admin
            }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
