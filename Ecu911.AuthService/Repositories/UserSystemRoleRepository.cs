using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class UserSystemRoleRepository : IUserSystemRoleRepository
{
    private readonly AuthDbContext _context;

    public UserSystemRoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid roleId, Guid systemModuleId)
    {
        return await _context.UserSystemRoles.AnyAsync(x =>
            x.UserId == userId &&
            x.RoleId == roleId &&
            x.SystemModuleId == systemModuleId);
    }

    public async Task AddAsync(UserSystemRole entity)
    {
        _context.UserSystemRoles.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserSystemRole>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSystemRoles
            .Include(x => x.Role)
            .Include(x => x.SystemModule)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}