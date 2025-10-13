namespace backend.Controllers;

using backend.Services;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : BaseController
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetReviewsByProduct(long productId)
    {
        // Validate product ID
        var idValidation = ValidateId(productId, "productId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.GetReviewsByProductIdAsync(productId);
        
        if (result.IsSuccess)
            return Ok(result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUser(long userId)
    {
        // Validate user ID
        var idValidation = ValidateId(userId, "userId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.GetReviewsByUserIdAsync(userId);
        
        if (result.IsSuccess)
            return Ok(result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user token" });

        var result = await _reviewService.GetReviewsByUserIdAsync(userId);
        
        if (result.IsSuccess)
            return Ok(result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReview(long reviewId)
    {
        // Validate review ID
        var idValidation = ValidateId(reviewId, "reviewId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.GetReviewByIdAsync(reviewId);
        
        if (result.IsSuccess)
            return Ok(result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewDto)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user token" });

        // Validate product ID in the request
        var idValidation = ValidateId(createReviewDto.ProductId, "productId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.CreateReviewAsync(userId, createReviewDto);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetReview), new { reviewId = result.Data!.Id }, result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpPut("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(long reviewId, [FromBody] UpdateReviewDto updateReviewDto)
    {
        // Validate review ID
        var idValidation = ValidateId(reviewId, "reviewId");
        if (idValidation != null)
            return idValidation;

        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user token" });

        var result = await _reviewService.UpdateReviewAsync(userId, reviewId, updateReviewDto);
        
        if (result.IsSuccess)
            return Ok(result.Data);
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpDelete("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(long reviewId)
    {
        // Validate review ID
        var idValidation = ValidateId(reviewId, "reviewId");
        if (idValidation != null)
            return idValidation;

        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new { error = "Invalid user token" });

        var result = await _reviewService.DeleteReviewAsync(userId, reviewId);
        
        if (result.IsSuccess)
            return NoContent();
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpGet("product/{productId}/average-rating")]
    public async Task<IActionResult> GetAverageRating(long productId)
    {
        // Validate product ID
        var idValidation = ValidateId(productId, "productId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.GetAverageRatingAsync(productId);
        
        if (result.IsSuccess)
            return Ok(new { AverageRating = result.Data });
        
        return HandleServiceError(result.ErrorMessage!);
    }

    [HttpGet("product/{productId}/count")]
    public async Task<IActionResult> GetReviewCount(long productId)
    {
        // Validate product ID
        var idValidation = ValidateId(productId, "productId");
        if (idValidation != null)
            return idValidation;

        var result = await _reviewService.GetReviewCountAsync(productId);
        
        if (result.IsSuccess)
            return Ok(new { ReviewCount = result.Data });
        
        return HandleServiceError(result.ErrorMessage!);
    }
}