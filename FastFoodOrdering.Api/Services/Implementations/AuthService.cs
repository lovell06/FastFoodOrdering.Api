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
    private readonly IEmailService _emailService;

    public AuthService(ApplicationDbContext dbContext, ITokenService tokenService, IPasswordService passwordService, IEmailService emailService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _emailService = emailService; // Gán giá trị
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

    public async Task<(bool IsSuccess, string Message)> SendRegistrationOtpAsync(SendOtpRequestDto request)
    {
        // 1. Kiểm tra Email đã tồn tại trong hệ thống chưa (AC4)
        var isEmailExist = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (isEmailExist)
        {
            return (false, "Email này đã được sử dụng. Vui lòng đăng nhập hoặc sử dụng email khác.");
        }

        // 2. Sinh mã OTP 6 số ngẫu nhiên
        string otpCode = new Random().Next(100000, 999999).ToString();

        // 3. Vô hiệu hóa các mã OTP cũ của email này (nếu có) để tránh spam/nhầm lẫn
        var oldOtps = await _dbContext.OtpVerifications
            .Where(o => o.Email == request.Email && !o.IsUsed)
            .ToListAsync();
        foreach (var oldOtp in oldOtps)
        {
            oldOtp.IsUsed = true;
        }

        // 4. Lưu OTP mới vào Database, thời hạn 5 phút
        var otpRecord = new OtpVerification
        {
            Email = request.Email,
            OtpCode = otpCode,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };
        _dbContext.OtpVerifications.Add(otpRecord);
        await _dbContext.SaveChangesAsync();

        // 5. Gửi Email (AC1)
        string emailBody = $"Mã xác thực đăng ký tài khoản của bạn là: {otpCode}. Mã này sẽ hết hạn sau 5 phút.";
        await _emailService.SendEmailAsync(request.Email, "Mã xác thực đăng ký", emailBody);

        return (true, "Mã OTP đã được gửi đến email của bạn.");
    }
}
