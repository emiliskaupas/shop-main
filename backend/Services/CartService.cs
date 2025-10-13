namespace backend.Services;

using backend.Data;
using backend.Models;
using backend.DTOs;
using backend.Mapping;
using Microsoft.EntityFrameworkCore;
using Shop.Shared.Results;
using Shop.Shared.Validation;

public class CartService : BaseService
{
    public CartService(AppDbContext shopContext) : base(shopContext)
    { }

    private async Task<Cart> GetOrCreateCartAsync(long userId)
    {
        var cart = await shopContext.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            
            await shopContext.Carts.AddAsync(cart);
            await shopContext.SaveChangesAsync();
        }

        return cart;
    }

    public async Task<Result<List<CartItemDto>>> GetCartItemsAsync(long userId)
    {
        try
        {
            // Validate user ID
            var idValidation = ValidateId<List<CartItemDto>>(userId, "userId");
            if (!idValidation.IsSuccess)
                return idValidation;

            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product!)
                        .ThenInclude(p => p.CreatedBy)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Result<List<CartItemDto>>.Success(new List<CartItemDto>());
            }

            var cartItemDtos = cart.CartItems.Select(ci => ci.ToDto()).ToList();
            return Result<List<CartItemDto>>.Success(cartItemDtos);
        }
        catch (Exception ex)
        {
            return Result<List<CartItemDto>>.Failure(ex);
        }
    }

    public async Task<Result<CartItemDto>> AddToCartAsync(long userId, long productId, int quantity)
    {
        try
        {
            // Validate IDs
            var idValidation = ValidateIds((userId, "userId"), (productId, "productId"));
            if (!idValidation.IsSuccess)
                return Result<CartItemDto>.Failure(idValidation.ErrorMessage!);
            
            // Validate inputs
            var quantityValidation = quantity.ValidateRange(1, 100, "Quantity");
            if (!quantityValidation.IsSuccess)
                return Result<CartItemDto>.Failure(quantityValidation.ErrorMessage!);

            // Check if product exists
            var product = await shopContext.Products.FindAsync(productId);
            if (product == null)
            {
                Console.WriteLine($"Product not found: productId={productId}");
                return Result<CartItemDto>.Failure("Product not found");
            }

            // Check if user exists
            var user = await shopContext.Users.FindAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"User not found: userId={userId}");
                return Result<CartItemDto>.Failure("User not found");
            }

            Console.WriteLine($"Validation passed. Creating/updating cart item...");
            
            // Get or create cart for user
            var cart = await GetOrCreateCartAsync(userId);
            
            // Check if item already exists in cart
            var existingCartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += quantity;
                await shopContext.SaveChangesAsync();
                
                // Reload with product and user information
                await shopContext.Entry(existingCartItem)
                    .Reference(ci => ci.Product)
                    .LoadAsync();
                    
                if (existingCartItem.Product != null)
                {
                    await shopContext.Entry(existingCartItem.Product)
                        .Reference(p => p.CreatedBy)
                        .LoadAsync();
                }
                
                return Result<CartItemDto>.Success(existingCartItem.ToDto());
            }
            else
            {
                // Create new cart item
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };

                cart.CartItems.Add(newCartItem);
                await shopContext.CartItems.AddAsync(newCartItem);
                await shopContext.SaveChangesAsync();
                
                // Reload with product and user information
                await shopContext.Entry(newCartItem)
                    .Reference(ci => ci.Product)
                    .LoadAsync();
                    
                if (newCartItem.Product != null)
                {
                    await shopContext.Entry(newCartItem.Product)
                        .Reference(p => p.CreatedBy)
                        .LoadAsync();
                }

                return Result<CartItemDto>.Success(newCartItem.ToDto());
            }
        }
        catch (Exception ex)
        {
            return Result<CartItemDto>.Failure(ex);
        }
    }

    public async Task<Result<CartItemDto>> UpdateCartItemQuantityAsync(long userId, long cartItemId, int quantity)
    {
        try
        {
            // Validate quantity
            var quantityValidation = quantity.ValidateRange(1, 100, "Quantity");
            if (!quantityValidation.IsSuccess)
                return Result<CartItemDto>.Failure(quantityValidation.ErrorMessage!);

            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product!)
                        .ThenInclude(p => p.CreatedBy)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return Result<CartItemDto>.Failure("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return Result<CartItemDto>.Failure("Cart item not found");

            // Check if user is trying to update quantity of their own product
            if (cartItem.Product != null && cartItem.Product.CreatedByUserId == userId)
                return Result<CartItemDto>.Failure("You cannot modify cart items containing your own products");

            cartItem.Quantity = quantity;
            await shopContext.SaveChangesAsync();

            return Result<CartItemDto>.Success(cartItem.ToDto());
        }
        catch (Exception ex)
        {
            return Result<CartItemDto>.Failure(ex);
        }
    }

    public async Task<Result> RemoveFromCartAsync(long userId, long cartItemId)
    {
        try
        {
            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return Result.Failure("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return Result.Failure("Cart item not found");

            cart.CartItems.Remove(cartItem);
            shopContext.CartItems.Remove(cartItem);
            await shopContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex);
        }
    }

    public async Task<Result> ClearCartAsync(long userId)
    {
        try
        {
            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.CartItems.Any())
            {
                shopContext.CartItems.RemoveRange(cart.CartItems);
                cart.CartItems.Clear();
                await shopContext.SaveChangesAsync();
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex);
        }
    }

    public async Task<Result<decimal>> GetCartTotalAsync(long userId)
    {
        try
        {
            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Result<decimal>.Success(0);
            }

            var total = cart.CartItems.Sum(ci => ci.Product!.Price * ci.Quantity);

            return Result<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Failure(ex);
        }
    }

    public async Task<Result<int>> GetCartItemCountAsync(long userId)
    {
        try
        {
            var cart = await shopContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Result<int>.Success(0);
            }

            var count = cart.CartItems.Sum(ci => ci.Quantity);

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex);
        }
    }
}