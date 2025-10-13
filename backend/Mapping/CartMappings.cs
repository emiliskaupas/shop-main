using backend.DTOs;
using backend.Models;

namespace backend.Mapping;

public static class CartMappings
{
    public static CartItemDto ToDto(this CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            UserId = cartItem.Cart?.UserId ?? 0,
            ProductId = cartItem.ProductId,
            Product = cartItem.Product?.ToDto(),
            Quantity = cartItem.Quantity
        };
    }
}