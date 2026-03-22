using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission?> GetByCodeAsync(string code);
    Task<List<Permission>> GetAllAsync();
    Task<Permission> AddAsync(Permission entity);
}