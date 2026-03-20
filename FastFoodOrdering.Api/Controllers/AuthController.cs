using FastFoodOrdering.Api.DTOs.Auth;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodOrdering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult?> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });

        return Ok(response);
    }
}
