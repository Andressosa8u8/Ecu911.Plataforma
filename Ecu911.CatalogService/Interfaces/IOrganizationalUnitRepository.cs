using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IOrganizationalUnitRepository
{
    Task<List<OrganizationalUnit>> GetAllAsync();
    Task<List<OrganizationalUnit>> GetRootsAsync();
    Task<List<OrganizationalUnit>> GetChildrenAsync(Guid parentId);
    Task<OrganizationalUnit?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null);
    Task<bool> HasActiveChildrenAsync(Guid id);
    Task<OrganizationalUnit> AddAsync(OrganizationalUnit entity);
    Task<OrganizationalUnit?> UpdateAsync(OrganizationalUnit entity);
    Task<bool> DeleteAsync(Guid id, string? username);
}