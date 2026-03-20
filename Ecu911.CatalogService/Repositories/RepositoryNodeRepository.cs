using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class RepositoryNodeRepository : IRepositoryNodeRepository
{
    private readonly AppDbContext _context;

    public RepositoryNodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RepositoryNode>> GetAllAsync()
    {
        return await _context.RepositoryNodes
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<RepositoryNode>> GetRootsAsync()
    {
        return await _context.RepositoryNodes
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.ParentId == null)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<RepositoryNode>> GetChildrenAsync(Guid parentId)
    {
        return await _context.RepositoryNodes
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.ParentId == parentId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<RepositoryNode?> GetByIdAsync(Guid id)
    {
        return await _context.RepositoryNodes
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.RepositoryNodes
            .AnyAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        return await _context.RepositoryNodes.AnyAsync(x =>
            !x.IsDeleted &&
            x.Name.ToLower() == name.ToLower() &&
            x.ParentId == parentId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<bool> HasActiveChildrenAsync(Guid id)
    {
        return await _context.RepositoryNodes
            .AnyAsync(x => !x.IsDeleted && x.ParentId == id);
    }

    public async Task<RepositoryNode> AddAsync(RepositoryNode entity)
    {
        _context.RepositoryNodes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<RepositoryNode?> UpdateAsync(RepositoryNode entity)
    {
        _context.RepositoryNodes.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.RepositoryNodes
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