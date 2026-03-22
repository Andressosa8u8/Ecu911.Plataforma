using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface ISystemModuleRepository
{
    Task<SystemModule?> GetByIdAsync(Guid id);
    Task<SystemModule?> GetByCodeAsync(string code);
    Task<List<SystemModule>> GetAllAsync();
    Task<SystemModule> AddAsync(SystemModule entity);
}