using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IRolePermissionRepository
{
    Task<bool> ExistsAsync(Guid roleId, Guid permissionId);
    Task AddAsync(RolePermission entity);
    Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
}