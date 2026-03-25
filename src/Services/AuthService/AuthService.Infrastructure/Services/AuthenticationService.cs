using AuthService.Application.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Services;

/// <summary>
/// Kimlik doğrulama servisi implementasyonu
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        // Email kontrolü
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Invalid credentials");
        }

        // Şifreyi BCrypt ile hashle
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Yeni User entity oluştur
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Kullanıcıyı repository'ye kaydet
        await _userRepository.CreateUserAsync(user);

        // LoginAsync çağır ve TokenResponse döndür
        var loginRequest = new LoginRequest { Email = request.Email, Password = request.Password };
        return await LoginAsync(loginRequest);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        // Email'e göre user bul
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Şifre doğrulama
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Token üret
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        // RefreshToken entity oluştur ve kaydet
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddRefreshTokenAsync(refreshToken);

        // TokenResponse döndür
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60"))
        };
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken)
    {
        // RefreshToken bul
        var token = await _userRepository.GetRefreshTokenAsync(refreshToken);

        // Token kontrolü: null, revoked veya expired
        if (token is null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Eski token'ı revoke et
        await _userRepository.RevokeRefreshTokenAsync(refreshToken);

        // User bul
        var user = await _userRepository.GetUserByIdAsync(token.UserId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Yeni token çifti üret
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenString = _tokenService.GenerateRefreshToken();

        // Yeni RefreshToken kaydet
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddRefreshTokenAsync(newRefreshToken);

        // Yeni TokenResponse döndür
        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60"))
        };
    }
}
