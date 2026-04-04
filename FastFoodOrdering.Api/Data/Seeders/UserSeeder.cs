using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Data.Seeders;

public static class UserSeeder
{
    private const string AdminEmail = "admin012@gmail.com";
    private const string AdminName = "admin";
    private const string AdminPhone = "0999999999";
    private const string AdminPassword = "Adminpassword123@";
    private const string IdentityPasswordHashPrefix = "AQAAAA";

    public static void Seed(ApplicationDbContext context)
    {
        var admin = context.Users.SingleOrDefault(u => u.Email == AdminEmail);
        if (admin == null)
        {
            context.Users.Add(CreateAdminUser());
        }
        else
        {
            ApplyAdminDefaults(admin);
        }

        context.SaveChanges();
    }

    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        var admin = await context.Users.SingleOrDefaultAsync(u => u.Email == AdminEmail, cancellationToken);
        if (admin == null)
        {
            await context.Users.AddAsync(CreateAdminUser(), cancellationToken);
        }
        else
        {
            ApplyAdminDefaults(admin);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static User CreateAdminUser()
    {
        var admin = new User
        {
            Email = AdminEmail
        };

        ApplyAdminDefaults(admin);
        return admin;
    }

    private static void ApplyAdminDefaults(User admin)
    {
        if (string.IsNullOrWhiteSpace(admin.FullName))
        {
            admin.FullName = AdminName;
        }

        if (string.IsNullOrWhiteSpace(admin.Phone))
        {
            admin.Phone = AdminPhone;
        }

        admin.Role = UserRole.Admin;

        if (NeedsPasswordMigration(admin.Password))
        {
            var passwordHasher = new PasswordHasher<User>();
            admin.Password = passwordHasher.HashPassword(admin, AdminPassword);
        }
    }

    private static bool NeedsPasswordMigration(string password)
    {
        return string.IsNullOrWhiteSpace(password)
            || !password.StartsWith(IdentityPasswordHashPrefix, StringComparison.Ordinal);
    }
}
