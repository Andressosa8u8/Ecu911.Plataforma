using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface INodePermissionService
{
    Task<List<NodePermissionDto>> GetAllAsync();
    Task<List<NodePermissionDto>> GetByNodeIdAsync(Guid repositoryNodeId);
    Task<List<NodePermissionDto>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId);
    Task<NodePermissionDto?> GetByIdAsync(Guid id);
    Task<NodePermissionDto> CreateAsync(CreateNodePermissionDto input, string? username);
    Task<NodePermissionDto?> UpdateAsync(Guid id, UpdateNodePermissionDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}