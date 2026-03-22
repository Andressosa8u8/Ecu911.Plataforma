using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class UserSystemScopeRepository : IUserSystemScopeRepository
{
    private readonly AuthDbContext _context;

    public UserSystemScopeRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserSystemScope?> GetByUserAndSystemAsync(Guid userId, Guid systemModuleId)
    {
        return await _context.UserSystemScopes
            .FirstOrDefaultAsync(x => x.UserId == userId && x.SystemModuleId == systemModuleId);
    }

    public async Task AddAsync(UserSystemScope entity)
    {
        _context.UserSystemScopes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserSystemScope entity)
    {
        _context.UserSystemScopes.Update(entity);
        await _context.SaveChangesAsync();
    }
}