using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using backend.DTOs;
using backend.Models;
using Shop.Shared.Results;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all cart operations
public class CartController : BaseController
{
    private readonly CartService cartService;

    public CartController(CartService cartService)
    {
        this.cartService = cartService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCartItems(long userId)
    {
        // Validate user ID
        var idValidation = ValidateId(userId, "userId");
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        var result = await this.cartService.GetCartItemsAsync(userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddToCart(long userId, [FromBody] AddToCartRequestDto request)
    {
        // Validate IDs
        var idValidation = ValidateIds((userId, "userId"), (request.ProductId, "productId"));
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        // Validate quantity
        if (request.Quantity <= 0)
        {
            return BadRequest(new { 
                error = "Invalid quantity. Must be a positive number.",
                field = "quantity",
                value = request.Quantity
            });
        }
        
        var result = await this.cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpPut("{userId}/items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(long userId, long cartItemId, [FromBody] UpdateQuantityRequestDto request)
    {
        // Validate IDs
        var idValidation = ValidateIds((userId, "userId"), (cartItemId, "cartItemId"));
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        // Validate quantity
        if (request.Quantity <= 0)
        {
            return BadRequest(new { 
                error = "Invalid quantity. Must be a positive number.",
                field = "quantity",
                value = request.Quantity
            });
        }

        var result = await this.cartService.UpdateCartItemQuantityAsync(userId, cartItemId, request.Quantity);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(result.Data);
    }

    [HttpDelete("{userId}/items/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(long userId, long cartItemId)
    {
        // Validate IDs
        var idValidation = ValidateIds((userId, "userId"), (cartItemId, "cartItemId"));
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        var result = await this.cartService.RemoveFromCartAsync(userId, cartItemId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return NoContent();
    }

    [HttpDelete("{userId}/clear")]
    public async Task<IActionResult> ClearCart(long userId)
    {
        // Validate user ID
        var idValidation = ValidateId(userId, "userId");
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        var result = await this.cartService.ClearCartAsync(userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return NoContent();
    }

    [HttpGet("{userId}/total")]
    public async Task<IActionResult> GetCartTotal(long userId)
    {
        // Validate user ID
        var idValidation = ValidateId(userId, "userId");
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        var result = await this.cartService.GetCartTotalAsync(userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(new { total = result.Data });
    }

    [HttpGet("{userId}/count")]
    public async Task<IActionResult> GetCartItemCount(long userId)
    {
        // Validate user ID
        var idValidation = ValidateId(userId, "userId");
        if (idValidation != null)
            return idValidation;

        // Verify that the user is accessing their own cart or is an admin
        if (!CanAccessUser(userId))
            return Forbid();

        var result = await this.cartService.GetCartItemCountAsync(userId);

        if (!result.IsSuccess)
            return HandleServiceError(result.ErrorMessage!);

        return Ok(new { count = result.Data });
    }
}
