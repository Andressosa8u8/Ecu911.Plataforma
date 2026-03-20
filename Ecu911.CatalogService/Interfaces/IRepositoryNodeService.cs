using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IRepositoryNodeService
{
    Task<List<RepositoryNodeDto>> GetAllAsync();
    Task<List<RepositoryNodeDto>> GetRootsAsync();
    Task<List<RepositoryNodeDto>> GetChildrenAsync(Guid parentId);
    Task<RepositoryNodeDto?> GetByIdAsync(Guid id);
    Task<RepositoryNodeDto> CreateAsync(CreateRepositoryNodeDto input, string? username);
    Task<RepositoryNodeDto?> UpdateAsync(Guid id, UpdateRepositoryNodeDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}