using Microsoft.AspNetCore.Mvc;
using FastFoodOrdering.Api.Services.Interfaces;

namespace FastFoodOrdering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductAsync();

        return Ok(products);
    }

}