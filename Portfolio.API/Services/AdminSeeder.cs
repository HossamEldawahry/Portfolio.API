using Microsoft.Extensions.Options;
using Portfolio.API.Authorization;
using System.Data.Common;

namespace Portfolio.API.Services;

public sealed class AdminSeeder : IAdminSeeder
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _passwordService;
    private readonly IOptions<AdminAccountOptions> _adminOptions;

    public AdminSeeder(AppDbContext db, IPasswordService passwordService, IOptions<AdminAccountOptions> adminOptions)
    {
        _db = db;
        _passwordService = passwordService;
        _adminOptions = adminOptions;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var config = _adminOptions.Value;
        if (string.IsNullOrWhiteSpace(config.Username))
            return;

        AdminUser? existing;
        try
        {
            existing = await _db.AdminUsers.SingleOrDefaultAsync(x => x.Username == config.Username, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (DbException)
        {
            // Database schema may not be migrated yet in test environments.
            return;
        }
        if (existing is not null)
            return;

        var hash = config.PasswordHash;
        if (string.IsNullOrWhiteSpace(hash))
        {
            if (string.IsNullOrWhiteSpace(config.Password))
                return;
            hash = _passwordService.HashPassword(config.Password);
        }

        _db.AdminUsers.Add(new AdminUser
        {
            Username = config.Username,
            PasswordHash = hash,
            IsActive = true
        });
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
