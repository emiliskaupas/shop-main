using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;
using backend.Services;
using Shop.Shared.Pagination;
using Shop.Shared.Results;
namespace backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly ProductService productService;

    public ProductsController(ProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] PaginationRequest request)
    {
        var result = await this.productService.GetProductsAsync(request);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(long id)
    {
        // Validate ID
        var idValidation = ValidateId(id, "productId");
        if (idValidation != null)
            return idValidation;

        var result = await this.productService.GetProductByIdAsync(id);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpGet("my-products")]
    [Authorize] // Require authentication to see own products
    public async Task<IActionResult> GetMyProducts([FromQuery] PaginationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user authentication" });

        var result = await this.productService.GetProductsByUserAsync(userId, request);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize] // Require authentication for creating products
    public async Task<IActionResult> AddProduct([FromBody] CreateProductDto product)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user authentication" });

        var result = await this.productService.AddProductAsync(product, userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return CreatedAtAction(nameof(GetProductById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize] // Require authentication for updating products
    public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto product)
    {
        // Validate ID
        var idValidation = ValidateId(id, "productId");
        if (idValidation != null)
            return idValidation;

        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user authentication" });

        var result = await this.productService.UpdateProductAsync(id, product, userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize] // Require authentication for deleting products
    public async Task<IActionResult> DeleteProduct(long id)
    {
        // Validate ID
        var idValidation = ValidateId(id, "productId");
        if (idValidation != null)
            return idValidation;

        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user authentication" });

        var result = await this.productService.DeleteProductAsync(id, userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return NoContent();
    }
}
