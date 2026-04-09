namespace Portfolio.API.DTOs;

public sealed class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
