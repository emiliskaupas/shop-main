namespace backend.DTOs;

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
