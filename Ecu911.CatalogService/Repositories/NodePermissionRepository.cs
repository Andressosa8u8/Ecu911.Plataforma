using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class NodePermissionRepository : INodePermissionRepository
{
    private readonly AppDbContext _context;

    public NodePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NodePermission>> GetAllAsync()
    {
        return await _context.NodePermissions
            .Include(x => x.RepositoryNode)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.RepositoryNode!.Name)
            .ThenBy(x => x.OrganizationalUnit!.Name)
            .ToListAsync();
    }

    public async Task<List<NodePermission>> GetByNodeIdAsync(Guid repositoryNodeId)
    {
        return await _context.NodePermissions
            .Include(x => x.RepositoryNode)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.RepositoryNodeId == repositoryNodeId)
            .OrderBy(x => x.OrganizationalUnit!.Name)
            .ToListAsync();
    }

    public async Task<List<NodePermission>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId)
    {
        return await _context.NodePermissions
            .Include(x => x.RepositoryNode)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.OrganizationalUnitId == organizationalUnitId)
            .OrderBy(x => x.RepositoryNode!.Name)
            .ToListAsync();
    }

    public async Task<NodePermission?> GetByIdAsync(Guid id)
    {
        return await _context.NodePermissions
            .Include(x => x.RepositoryNode)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid repositoryNodeId, Guid organizationalUnitId, Guid? excludeId = null)
    {
        return await _context.NodePermissions.AnyAsync(x =>
            !x.IsDeleted &&
            x.RepositoryNodeId == repositoryNodeId &&
            x.OrganizationalUnitId == organizationalUnitId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<NodePermission> AddAsync(NodePermission entity)
    {
        _context.NodePermissions.Add(entity);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id) ?? entity;
    }

    public async Task<NodePermission?> UpdateAsync(NodePermission entity)
    {
        _context.NodePermissions.Update(entity);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.NodePermissions
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

    public async Task<NodePermission?> GetByNodeIdAndOrganizationalUnitIdAsync(Guid repositoryNodeId, Guid organizationalUnitId)
    {
        return await _context.NodePermissions
            .Include(x => x.RepositoryNode)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.RepositoryNodeId == repositoryNodeId &&
                x.OrganizationalUnitId == organizationalUnitId);
    }

    public async Task<List<Guid>> GetReadableNodeIdsByOrganizationalUnitIdAsync(Guid organizationalUnitId)
    {
        return await _context.NodePermissions
            .Where(x =>
                !x.IsDeleted &&
                x.OrganizationalUnitId == organizationalUnitId &&
                (x.CanView || x.CanManage))
            .Select(x => x.RepositoryNodeId)
            .Distinct()
            .ToListAsync();
    }
}