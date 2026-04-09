namespace Portfolio.API.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [MaxLength(128)]
    public string TokenHash { get; set; } = string.Empty;

    [MaxLength(100)]
    public string JwtId { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
    public bool IsRevoked { get; set; }

    public int AdminUserId { get; set; }
    public AdminUser AdminUser { get; set; } = null!;
}
