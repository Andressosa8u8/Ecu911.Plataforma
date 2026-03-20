using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IRepositoryNodeRepository
{
    Task<List<RepositoryNode>> GetAllAsync();
    Task<List<RepositoryNode>> GetRootsAsync();
    Task<List<RepositoryNode>> GetChildrenAsync(Guid parentId);
    Task<RepositoryNode?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null);
    Task<bool> HasActiveChildrenAsync(Guid id);
    Task<RepositoryNode> AddAsync(RepositoryNode entity);
    Task<RepositoryNode?> UpdateAsync(RepositoryNode entity);
    Task<bool> DeleteAsync(Guid id, string? username);
}