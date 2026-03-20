using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class OrganizationalUnitRepository : IOrganizationalUnitRepository
{
    private readonly AppDbContext _context;

    public OrganizationalUnitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationalUnit>> GetAllAsync()
    {
        return await _context.OrganizationalUnits
            .Include(x => x.Parent)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<OrganizationalUnit>> GetRootsAsync()
    {
        return await _context.OrganizationalUnits
            .Include(x => x.Parent)
            .Where(x => !x.IsDeleted && x.ParentId == null)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<OrganizationalUnit>> GetChildrenAsync(Guid parentId)
    {
        return await _context.OrganizationalUnits
            .Include(x => x.Parent)
            .Where(x => !x.IsDeleted && x.ParentId == parentId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<OrganizationalUnit?> GetByIdAsync(Guid id)
    {
        return await _context.OrganizationalUnits
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.OrganizationalUnits
            .AnyAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        return await _context.OrganizationalUnits.AnyAsync(x =>
            !x.IsDeleted &&
            x.Name.ToLower() == name.ToLower() &&
            x.ParentId == parentId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<bool> HasActiveChildrenAsync(Guid id)
    {
        return await _context.OrganizationalUnits
            .AnyAsync(x => !x.IsDeleted && x.ParentId == id);
    }

    public async Task<OrganizationalUnit> AddAsync(OrganizationalUnit entity)
    {
        _context.OrganizationalUnits.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<OrganizationalUnit?> UpdateAsync(OrganizationalUnit entity)
    {
        _context.OrganizationalUnits.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.OrganizationalUnits
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            return false;
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = username;

        await _context.SaveChangesAsync();
        return true;
    }
}