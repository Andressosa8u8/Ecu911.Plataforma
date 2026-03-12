using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Role> AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }
}