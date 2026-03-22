using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class SystemModuleRepository : ISystemModuleRepository
{
    private readonly AuthDbContext _context;

    public SystemModuleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<SystemModule?> GetByIdAsync(Guid id)
    {
        return await _context.SystemModules.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SystemModule?> GetByCodeAsync(string code)
    {
        return await _context.SystemModules
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<SystemModule>> GetAllAsync()
    {
        return await _context.SystemModules
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<SystemModule> AddAsync(SystemModule entity)
    {
        _context.SystemModules.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}