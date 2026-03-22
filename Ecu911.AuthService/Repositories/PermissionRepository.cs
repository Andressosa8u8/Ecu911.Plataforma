using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AuthDbContext _context;

    public PermissionRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Permission?> GetByCodeAsync(string code)
    {
        return await _context.Permissions.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Permission> AddAsync(Permission entity)
    {
        _context.Permissions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}