namespace AuthService.Domain.Interfaces;

/// <summary>
/// Kullanıcı repository interface'i
/// </summary>
public interface IUserRepository
{
    Task<AuthService.Domain.Entities.User?> GetUserByIdAsync(Guid id);
    Task<AuthService.Domain.Entities.User?> GetUserByUsernameAsync(string username);
    Task<AuthService.Domain.Entities.User?> GetUserByEmailAsync(string email);
    Task<AuthService.Domain.Entities.User> CreateUserAsync(AuthService.Domain.Entities.User user);
    Task<AuthService.Domain.Entities.User> UpdateUserAsync(AuthService.Domain.Entities.User user);
    Task DeleteUserAsync(Guid id);
}
