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
    public AuthService(ApplicationDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return null;

        if (user.Password != request.Password)
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

        // 2. Mã hóa mật khẩu
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Tạo User mới (Happy Path)
        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Password = passwordHash,
            Role = UserRole.Customer
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return (true, "Đăng ký thành công! Vui lòng kiểm tra email để kích hoạt tài khoản.");
    }
}
