using backend.DTOs;
using backend.Models;

namespace backend.Mapping;

public static class ReviewMappings
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            Username = review.User?.Username ?? string.Empty,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ModifiedAt = review.ModifiedAt
        };
    }
}