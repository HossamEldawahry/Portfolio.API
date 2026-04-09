namespace Portfolio.API.Authorization;

public sealed class AdminAccountOptions
{
    public const string SectionName = "AdminAccount";

    public string Username { get; set; } = "admin";
    public string Password { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
}
