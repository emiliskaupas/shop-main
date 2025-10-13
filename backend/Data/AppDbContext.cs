using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Enums;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ============================
        // USER → CART (One-to-One)
        // ============================
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.User) // ✅ explicitly link back to User
            .HasForeignKey<Cart>(c => c.UserId)
            .IsRequired()         // ✅ ensures every user must have a cart
            .OnDelete(DeleteBehavior.Cascade);

        // ============================
        // CART → CART ITEMS (One-to-Many)
        // ============================
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================
        // CART ITEM → PRODUCT (Many-to-One)
        // ============================
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================
        // USER → PRODUCTS (CreatedBy) (One-to-Many)
        // ============================
        modelBuilder.Entity<Product>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================
        // CUSTOM CONFIG + SEED DATA
        // ============================
        modelBuilder.ConfigureEntities();
        modelBuilder.Seed();
    }
}
