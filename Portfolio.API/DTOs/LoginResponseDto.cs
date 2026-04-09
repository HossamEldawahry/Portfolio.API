namespace Portfolio.API.DTOs;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonIgnore]
    public string AccessTokenId { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAtUtc { get; set; }
}
