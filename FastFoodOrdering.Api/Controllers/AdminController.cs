using FastFoodOrdering.Api.Authorization;
using FastFoodOrdering.Api.DTOs.Product;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodOrdering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AdminOnly]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("bootstrap")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetBootstrap()
    {
        var data = await _adminService.GetBootstrapAsync();
        return Ok(data);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _adminService.GetProductsAsync();
        return Ok(products);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _adminService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _adminService.GetUsersAsync();
        return Ok(users);
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.Image == null || request.Image.Length == 0)
        {
            return BadRequest(new
            {
                message = "Image is required. Upload image file using multipart/form-data."
            });
        }

        var product = await _adminService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _adminService.UpdateProductAsync(id, request);
        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return Ok(product);
    }

    [HttpPatch("products/{id}/availability")]
    public async Task<IActionResult> UpdateProductAvailability(int id, [FromBody] UpdateProductAvailabilityRequestDto request)
    {
        var product = await _adminService.UpdateProductAvailabilityAsync(id, request);
        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return Ok(product);
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _adminService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return NoContent();
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _adminService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return Ok(product);
    }
}
