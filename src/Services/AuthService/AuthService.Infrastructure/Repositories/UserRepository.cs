using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Repositories;

/// <summary>
/// Kullanıcı Repository implementasyonu
/// </summary>
public class UserRepository : IUserRepository
{
    // private readonly AuthDbContext _context;

    // public UserRepository(AuthDbContext context)
    // {
    //     _context = context;
    // }

    public Task<User?> GetUserByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User> CreateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
    }
}
