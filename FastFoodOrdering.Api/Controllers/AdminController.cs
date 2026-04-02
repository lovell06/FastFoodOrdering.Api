using Microsoft.AspNetCore.Mvc;
using FastFoodOrdering.Api.Services.Interfaces;
using FastFoodOrdering.Api.DTOs.Product;

namespace FastFoodOrdering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // POST /api/admin/products
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _adminService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    // PUT /api/admin/products/{id}
    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _adminService.UpdateProductAsync(id, request);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // DELETE /api/admin/products/{id}
    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _adminService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET /api/admin/products/{id}
    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _adminService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}