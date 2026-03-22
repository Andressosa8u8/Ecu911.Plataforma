using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IUserSystemScopeRepository
{
    Task<UserSystemScope?> GetByUserAndSystemAsync(Guid userId, Guid systemModuleId);
    Task AddAsync(UserSystemScope entity);
    Task UpdateAsync(UserSystemScope entity);
}