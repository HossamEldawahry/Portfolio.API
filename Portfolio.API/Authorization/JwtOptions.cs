namespace Portfolio.API.Authorization;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Portfolio.API";
    public string Audience { get; set; } = "Portfolio.Client";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 120;
    public int RefreshTokenDays { get; set; } = 7;
}
