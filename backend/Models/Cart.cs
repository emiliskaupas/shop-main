namespace backend.Models;

public class Cart : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
}