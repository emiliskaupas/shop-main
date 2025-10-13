namespace backend.Models;

public class Review : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}