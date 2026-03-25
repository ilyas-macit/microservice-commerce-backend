namespace AuthService.Application.Interfaces;

using AuthService.Application.DTOs;

/// <summary>
/// Kimlik doğrulama servisi interface'i
/// </summary>
public interface IAuthenticationService
{
    Task<TokenResponse> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshAsync(string refreshToken);
}
