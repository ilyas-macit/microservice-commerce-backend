namespace AuthService.Application.Interfaces;

using AuthService.Domain.Entities;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
