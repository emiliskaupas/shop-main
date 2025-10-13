using backend.DTOs;
using backend.Models;

namespace backend.Mapping;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product)
    {
        var reviews = product.Reviews?.Select(r => r.ToDto()).ToList() ?? new List<ReviewDto>();
        var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
        
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            ProductType = product.ProductType,
            CreatedByUserId = product.CreatedByUserId,
            CreatedByUserName = product.CreatedBy.Username,
            CreatedByEmail = product.CreatedBy.Email,
            CreatedAt = product.CreatedAt,
            ModifiedAt = product.ModifiedAt,
            Reviews = reviews,
            AverageRating = averageRating,
            ReviewCount = reviews.Count
        };
    }

    public static Product ToEntity(this CreateProductDto dto, long createdByUserId)
    {
        return new Product
        {
            Name = dto.Name,
            ShortDescription = dto.ShortDescription,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            ProductType = dto.ProductType,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateProductDto dto, Product product)
    {
        product.Name = dto.Name;
        product.ShortDescription = dto.ShortDescription;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.ProductType = dto.ProductType;
    }
}