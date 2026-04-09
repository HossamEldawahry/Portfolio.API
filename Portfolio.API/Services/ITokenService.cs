namespace Portfolio.API.Services;

public interface ITokenService
{
    LoginResponseDto CreateAdminToken(string username);
}
