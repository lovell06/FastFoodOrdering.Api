using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.DTOs.Auth;
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
}
