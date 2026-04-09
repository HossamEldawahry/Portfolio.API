namespace Portfolio.API.Services;

public interface IAdminSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
