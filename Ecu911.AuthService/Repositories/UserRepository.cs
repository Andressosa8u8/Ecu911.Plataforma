using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .OrderBy(x => x.FullName)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}