using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FastFoodOrdering.Api.Services.Implementations;

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public PasswordService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    public string HashPassword(User user, string password)
    {
        string hashedPassword = _passwordHasher.HashPassword(user, password);
        return hashedPassword;
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
