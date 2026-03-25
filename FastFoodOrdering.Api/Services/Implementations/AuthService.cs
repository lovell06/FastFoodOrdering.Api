using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Auth;
using FastFoodOrdering.Api.Enums;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOrdering.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    public AuthService(ApplicationDbContext dbContext, ITokenService tokenService, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return null;

        if (!_passwordService.VerifyPassword(user, user.Password, request.Password))
            return null;

        var token = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Kiểm tra Email (Unhappy Path)
        var isEmailExist = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (isEmailExist)
        {
            return (false, "Email này đã được sử dụng. Vui lòng đăng nhập hoặc sử dụng email khác.");
        }

        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Role = UserRole.Customer
        };
        // 2. Mã hóa mật khẩu
        string passwordHash = _passwordService.HashPassword(newUser, request.Password);
        newUser.Password = passwordHash;

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return (true, "Đăng ký thành công! Vui lòng kiểm tra email để kích hoạt tài khoản.");
    }
}
