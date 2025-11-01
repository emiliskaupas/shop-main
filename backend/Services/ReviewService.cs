namespace backend.Services;

using backend.Data;
using backend.Models;
using backend.DTOs;
using backend.Mapping;
using Microsoft.EntityFrameworkCore;
using Shop.Shared.Results;
using Shop.Shared.Validation;

public class ReviewService : BaseService
{
    public ReviewService(AppDbContext shopContext) : base(shopContext)
    { }

    public async Task<Result<List<ReviewDto>>> GetReviewsByProductIdAsync(long productId)
    {
        try
        {
            var reviews = await shopContext.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewDtos = reviews.Select(r => r.ToDto()).ToList();
            return Result<List<ReviewDto>>.Success(reviewDtos);
        }
        catch (Exception ex)
        {
            return Result<List<ReviewDto>>.Failure(ex);
        }
    }

    public async Task<Result<List<ReviewDto>>> GetReviewsByUserIdAsync(long userId)
    {
        try
        {
            var reviews = await shopContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewDtos = reviews.Select(r => r.ToDto()).ToList();
            return Result<List<ReviewDto>>.Success(reviewDtos);
        }
        catch (Exception ex)
        {
            return Result<List<ReviewDto>>.Failure(ex);
        }
    }

    public async Task<Result<ReviewDto>> GetReviewByIdAsync(long reviewId)
    {
        try
        {
            var review = await shopContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return Result<ReviewDto>.Failure("Review not found");

            return Result<ReviewDto>.Success(review.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure(ex);
        }
    }

    public async Task<Result<ReviewDto>> CreateReviewAsync(long userId, CreateReviewDto createReviewDto)
    {
        try
        {
            // Validate rating
            var ratingValidation = createReviewDto.Rating.ValidateRange(1, 5, "Rating");
            if (!ratingValidation.IsSuccess)
                return Result<ReviewDto>.Failure(ratingValidation.ErrorMessage!);

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(createReviewDto.Comment))
            {
                var commentValidation = createReviewDto.Comment.ValidateMaxLength(1000, "Comment");
                if (!commentValidation.IsSuccess)
                    return Result<ReviewDto>.Failure(commentValidation.ErrorMessage!);
            }

            // Check if product exists
            var product = await shopContext.Products.FindAsync(createReviewDto.ProductId);
            if (product == null)
                return Result<ReviewDto>.Failure("Product not found");

            // Check if user exists
            var user = await shopContext.Users.FindAsync(userId);
            if (user == null)
                return Result<ReviewDto>.Failure("User not found");

            // Check if user is trying to review their own product
            if (product.CreatedByUserId == userId)
                return Result<ReviewDto>.Failure("You cannot review your own product");

            // Check if user has already reviewed this product
            var existingReview = await shopContext.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == createReviewDto.ProductId);

            if (existingReview != null)
                return Result<ReviewDto>.Failure("You have already reviewed this product");

            // Create new review
            var newReview = new Review
            {
                UserId = userId,
                ProductId = createReviewDto.ProductId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await shopContext.Reviews.AddAsync(newReview);
            await shopContext.SaveChangesAsync();

            // Reload with user information
            await shopContext.Entry(newReview)
                .Reference(r => r.User)
                .LoadAsync();

            return Result<ReviewDto>.Success(newReview.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure(ex);
        }
    }

    public async Task<Result<ReviewDto>> UpdateReviewAsync(long userId, long reviewId, UpdateReviewDto updateReviewDto, bool isAdmin = false)
    {
        try
        {
            // Validate rating
            var ratingValidation = updateReviewDto.Rating.ValidateRange(1, 5, "Rating");
            if (!ratingValidation.IsSuccess)
                return Result<ReviewDto>.Failure(ratingValidation.ErrorMessage!);

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(updateReviewDto.Comment))
            {
                var commentValidation = updateReviewDto.Comment.ValidateMaxLength(1000, "Comment");
                if (!commentValidation.IsSuccess)
                    return Result<ReviewDto>.Failure(commentValidation.ErrorMessage!);
            }

            var review = await shopContext.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return Result<ReviewDto>.Failure("Review not found");

            // Check if user owns this review or is admin
            if (!isAdmin && review.UserId != userId)
                return Result<ReviewDto>.Failure("You don't have permission to edit this review");

            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;
            review.ModifiedAt = DateTime.UtcNow;

            await shopContext.SaveChangesAsync();

            return Result<ReviewDto>.Success(review.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure(ex);
        }
    }

    public async Task<Result> DeleteReviewAsync(long userId, long reviewId, bool isAdmin = false)
    {
        try
        {
            var review = await shopContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return Result.Failure("Review not found");

            // Check if user owns this review or is admin
            if (!isAdmin && review.UserId != userId)
                return Result.Failure("You don't have permission to delete this review");

            shopContext.Reviews.Remove(review);
            await shopContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex);
        }
    }

    public async Task<Result<double>> GetAverageRatingAsync(long productId)
    {
        try
        {
            var reviews = await shopContext.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
                return Result<double>.Success(0.0);

            var averageRating = reviews.Average(r => r.Rating);
            return Result<double>.Success(averageRating);
        }
        catch (Exception ex)
        {
            return Result<double>.Failure(ex);
        }
    }

    public async Task<Result<int>> GetReviewCountAsync(long productId)
    {
        try
        {
            var count = await shopContext.Reviews
                .CountAsync(r => r.ProductId == productId);

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex);
        }
    }
}