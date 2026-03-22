using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AuthDbContext _context;

    public RolePermissionRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId)
    {
        return await _context.RolePermissions.AnyAsync(x =>
            x.RoleId == roleId &&
            x.PermissionId == permissionId);
    }

    public async Task AddAsync(RolePermission entity)
    {
        _context.RolePermissions.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Include(x => x.Permission)
            .Where(x => x.RoleId == roleId)
            .ToListAsync();
    }
}