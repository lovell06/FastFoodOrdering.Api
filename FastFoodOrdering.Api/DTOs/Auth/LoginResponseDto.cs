using FastFoodOrdering.Api.Enums;

namespace FastFoodOrdering.Api.DTOs.Auth;

public class LoginResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
}
