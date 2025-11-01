namespace backend.Models;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
}
