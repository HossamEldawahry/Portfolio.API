namespace Portfolio.API.DTOs;

public sealed class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
