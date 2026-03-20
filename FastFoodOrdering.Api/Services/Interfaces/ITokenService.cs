using FastFoodOrdering.Api.Models;

namespace FastFoodOrdering.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
