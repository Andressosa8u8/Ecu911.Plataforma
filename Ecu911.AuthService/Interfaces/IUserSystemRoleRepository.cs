using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IUserSystemRoleRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid roleId, Guid systemModuleId);
    Task AddAsync(UserSystemRole entity);
    Task<List<UserSystemRole>> GetByUserIdAsync(Guid userId);
}