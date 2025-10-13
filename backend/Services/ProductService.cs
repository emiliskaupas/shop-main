namespace backend.Services;

using backend.Data;
using backend.Models;
using backend.DTOs;
using backend.Mapping;
using Microsoft.EntityFrameworkCore;
using Shop.Shared.Results;
using Shop.Shared.Pagination;
using Shop.Shared.Validation;

public class ProductService : BaseService
{
    public ProductService(AppDbContext shopContext) : base(shopContext)
    { }

    public async Task<Result<PagedResult<ProductDto>>> GetProductsAsync(PaginationRequest request)
    {
        try
        {
            request.Validate();
            
            var totalCount = await shopContext.Products.CountAsync();
            var products = await shopContext.Products
                .Include(p => p.CreatedBy)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync();

            var productDtos = products.Select(p => p.ToDto()).ToList();
            var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResult<ProductDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductDto>>.Failure(ex);
        }
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(long id)
    {
        try
        {
            // Validate ID
            var idValidation = ValidateId<ProductDto>(id, "productId");
            if (!idValidation.IsSuccess)
                return idValidation;

            var product = await shopContext.Products
                .Include(p => p.CreatedBy)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null)
                return Result<ProductDto>.Failure("Product not found");
            
            return Result<ProductDto>.Success(product.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure(ex);
        }
    }

    public async Task<Result<PagedResult<ProductDto>>> GetProductsByUserAsync(long userId, PaginationRequest request)
    {
        try
        {
            // Validate user ID
            var idValidation = ValidateId<PagedResult<ProductDto>>(userId, "userId");
            if (!idValidation.IsSuccess)
                return idValidation;

            request.Validate();
            
            var totalCount = await shopContext.Products
                .Where(p => p.CreatedByUserId == userId)
                .CountAsync();
                
            var products = await shopContext.Products
                .Include(p => p.CreatedBy)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Where(p => p.CreatedByUserId == userId)
                .Skip(request.Skip)
                .Take(request.Take)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productDtos = products.Select(p => p.ToDto()).ToList();
            var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResult<ProductDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductDto>>.Failure(ex);
        }
    }

    public async Task<Result<ProductDto>> AddProductAsync(CreateProductDto productDto, long createdByUserId)
    {
        try
        {
            // Validate using shared validation extensions
            var nameValidation = productDto.Name.ValidateNotEmpty("Product name");
            if (!nameValidation.IsSuccess)
                return Result<ProductDto>.Failure(nameValidation.ErrorMessage!);

            var priceValidation = productDto.Price.ValidatePositive("Product price");
            if (!priceValidation.IsSuccess)
                return Result<ProductDto>.Failure(priceValidation.ErrorMessage!);

            var newProduct = productDto.ToEntity(createdByUserId);
            
            await shopContext.Products.AddAsync(newProduct);
            await shopContext.SaveChangesAsync();
            
            // Reload with CreatedBy relationship for proper DTO mapping
            var productWithUser = await shopContext.Products
                .Include(p => p.CreatedBy)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == newProduct.Id);
                
            return Result<ProductDto>.Success(productWithUser!.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure(ex);
        }
    }
    
    public async Task<Result<ProductDto>> UpdateProductAsync(long id, UpdateProductDto productDto, long userId)
    {
        try
        {
            // Validate IDs
            var idValidation = ValidateIds((id, "productId"), (userId, "userId"));
            if (!idValidation.IsSuccess)
                return Result<ProductDto>.Failure(idValidation.ErrorMessage!);

            var existingProduct = await shopContext.Products
                .Include(p => p.CreatedBy)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
                return Result<ProductDto>.Failure("Product not found");

            // Check if the user owns this product
            if (existingProduct.CreatedByUserId != userId)
                return Result<ProductDto>.Failure("You can only modify your own products");

            // Validate using shared validation extensions
            var nameValidation = productDto.Name.ValidateNotEmpty("Product name");
            if (!nameValidation.IsSuccess)
                return Result<ProductDto>.Failure(nameValidation.ErrorMessage!);

            var priceValidation = productDto.Price.ValidatePositive("Product price");
            if (!priceValidation.IsSuccess)
                return Result<ProductDto>.Failure(priceValidation.ErrorMessage!);

            productDto.UpdateEntity(existingProduct);
            existingProduct.ModifiedAt = DateTime.UtcNow;

            await shopContext.SaveChangesAsync();
            return Result<ProductDto>.Success(existingProduct.ToDto());
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure(ex);
        }
    }

    public async Task<Result> DeleteProductAsync(long id, long userId)
    {
        try
        {
            // Validate IDs
            var idValidation = ValidateIds((id, "productId"), (userId, "userId"));
            if (!idValidation.IsSuccess)
                return idValidation;

            var product = await shopContext.Products.FindAsync(id);
            if (product == null)
                return Result.Failure("Product not found");

            // Check if the user owns this product
            if (product.CreatedByUserId != userId)
                return Result.Failure("You can only delete your own products");

            shopContext.Products.Remove(product);
            await shopContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex);
        }
    }
}
