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
        // 1. Check trùng (Chặn sớm)
        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
            return (false, "Email đã tồn tại.");

        // 2. Check OTP (Chặn sớm)
        var validOtp = await _dbContext.OtpVerifications
            .Where(o => o.Email == request.Email && o.OtpCode == request.OtpCode && !o.IsUsed)
            .FirstOrDefaultAsync();

        if (validOtp == null || validOtp.ExpiryTime < DateTime.UtcNow)
            return (false, "Mã OTP không hợp lệ hoặc hết hạn."); // <--- NẾU SAI OTP, CODE DỪNG TẠI ĐÂY.

        // 3. TỚI ĐÂY LÀ OTP ĐÃ ĐÚNG RỒI -> BẮT ĐẦU TẠO ACC
        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Role = UserRole.Customer,
            Password = _passwordService.HashPassword(new User(), request.Password)
        };

        // Đưa User vào danh sách chờ thêm
        _dbContext.Users.Add(newUser);

        // Đánh dấu mã OTP này đã dùng
        validOtp.IsUsed = true;
        _dbContext.OtpVerifications.Update(validOtp);

        // CHỐT HẠ: Lưu cả việc tạo User và việc cập nhật OTP vào DB cùng lúc
        await _dbContext.SaveChangesAsync();

        return (true, "Đăng ký thành công!");
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

    public async Task<(bool IsSuccess, string Message)> SendForgotPasswordOtpAsync(string email)
    {
        // 1. Kiểm tra Email xem có trong hệ thống không
        var user = await _dbContext.Users.AnyAsync(u => u.Email == email);
        if (!user) return (false, "Email này không tồn tại trong hệ thống.");

        // 2. Sinh mã OTP 6 số
        string otpCode = new Random().Next(100000, 999999).ToString();

        // 3. Lưu vào DB (Tái sử dụng bảng OtpVerifications)
        var otpRecord = new OtpVerification
        {
            Email = email,
            OtpCode = otpCode,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5), // Hiệu lực 5 phút theo AC5
            IsUsed = false
        };
        _dbContext.OtpVerifications.Add(otpRecord);
        await _dbContext.SaveChangesAsync();

        // 4. Gửi mail
        await _emailService.SendEmailAsync(email, "Mã đặt lại mật khẩu", $"Mã OTP của bạn là: {otpCode}");
        return (true, "Mã OTP đã được gửi.");
    }

    public async Task<(bool IsSuccess, string Message)> ResetPasswordAsync(ResetPasswordDto request)
    {
        // 1. Xác thực mã OTP (Giống luồng Register)
        var validOtp = await _dbContext.OtpVerifications
            .Where(o => o.Email == request.Email && o.OtpCode == request.OtpCode && !o.IsUsed)
            .FirstOrDefaultAsync();

        if (validOtp == null || validOtp.ExpiryTime < DateTime.UtcNow)
            return (false, "Mã OTP không chính xác hoặc đã hết hạn.");

        // 2. Tìm User để cập nhật
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return (false, "Lỗi hệ thống: Không tìm thấy người dùng.");

        // 3. Hash mật khẩu mới và lưu
        user.Password = _passwordService.HashPassword(user, request.NewPassword);

        // 4. Vô hiệu hóa OTP (AC5)
        validOtp.IsUsed = true;
        _dbContext.Update(user);
        _dbContext.Update(validOtp);

        await _dbContext.SaveChangesAsync();
        return (true, "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.");
    }
}
