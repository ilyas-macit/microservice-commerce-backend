using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

/// <summary>
/// Kullanıcı Repository implementasyonu
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserByIdAsync(Guid id)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
        {
            return;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _context.Set<RefreshToken>().Add(token);
        await _context.SaveChangesAsync();
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return _context.Set<RefreshToken>().FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.Set<RefreshToken>().FirstOrDefaultAsync(x => x.Token == token);
        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
            _context.Set<RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
