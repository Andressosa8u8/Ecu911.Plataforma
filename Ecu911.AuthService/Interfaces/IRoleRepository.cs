using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByIdAsync(Guid id);
    Task<List<Role>> GetAllAsync();
    Task<Role> AddAsync(Role role);
}