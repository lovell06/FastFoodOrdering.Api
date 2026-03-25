using FastFoodOrdering.Api.DTOs.Auth;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequestDto request);
}
