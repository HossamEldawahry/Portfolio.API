using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.API.Authorization;

namespace Portfolio.API.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        AppDbContext db,
        ITokenService tokenService,
        IPasswordService passwordService,
        IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _db.AdminUsers.SingleOrDefaultAsync(
            x => x.Username == request.Username && x.IsActive,
            cancellationToken).ConfigureAwait(false);
        if (user is null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            return null;

        return await IssueTokensAsync(user.Id, user.Username, cancellationToken).ConfigureAwait(false);
    }

    public async Task<LoginResponseDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeSha256(refreshToken);

        var current = await _db.RefreshTokens
            .Include(x => x.AdminUser)
            .SingleOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     !x.IsRevoked &&
                     x.ExpiresAtUtc > DateTime.UtcNow &&
                     x.AdminUser.IsActive,
                cancellationToken)
            .ConfigureAwait(false);

        if (current is null)
            return null;

        current.IsRevoked = true;
        current.RevokedAtUtc = DateTime.UtcNow;

        return await IssueTokensAsync(current.AdminUserId, current.AdminUser.Username, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeSha256(refreshToken);
        var current = await _db.RefreshTokens.SingleOrDefaultAsync(
            x => x.TokenHash == tokenHash && !x.IsRevoked,
            cancellationToken).ConfigureAwait(false);

        if (current is null)
            return false;

        current.IsRevoked = true;
        current.RevokedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task<LoginResponseDto> IssueTokensAsync(int adminUserId, string username, CancellationToken cancellationToken)
    {
        var pair = _tokenService.CreateAdminToken(username);
        var refreshTokenHash = ComputeSha256(pair.RefreshToken);

        _db.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = refreshTokenHash,
            JwtId = pair.AccessTokenId,
            AdminUserId = adminUserId,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            IsRevoked = false
        });

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return pair;
    }

    private static string ComputeSha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
