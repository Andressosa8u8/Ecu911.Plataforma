using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface INodePermissionRepository
{
    Task<List<NodePermission>> GetAllAsync();
    Task<List<NodePermission>> GetByNodeIdAsync(Guid repositoryNodeId);
    Task<List<NodePermission>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId);
    Task<NodePermission?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid repositoryNodeId, Guid organizationalUnitId, Guid? excludeId = null);
    Task<NodePermission> AddAsync(NodePermission entity);
    Task<NodePermission?> UpdateAsync(NodePermission entity);
    Task<bool> DeleteAsync(Guid id, string? username);
    Task<NodePermission?> GetByNodeIdAndOrganizationalUnitIdAsync(Guid repositoryNodeId, Guid organizationalUnitId);
    Task<List<Guid>> GetReadableNodeIdsByOrganizationalUnitIdAsync(Guid organizationalUnitId);
}