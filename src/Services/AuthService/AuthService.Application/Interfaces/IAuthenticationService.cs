namespace AuthService.Application.Interfaces;

/// <summary>
/// Kimlik doğrulama servisi interface'i
/// </summary>
public interface IAuthenticationService
{
    Task<string> GenerateTokenAsync(int userId, string username);
    Task<bool> ValidateTokenAsync(string token);
    Task LogoutAsync(string token);
}
