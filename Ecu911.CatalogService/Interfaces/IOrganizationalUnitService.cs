using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IOrganizationalUnitService
{
    Task<List<OrganizationalUnitDto>> GetAllAsync();
    Task<List<OrganizationalUnitDto>> GetRootsAsync();
    Task<List<OrganizationalUnitDto>> GetChildrenAsync(Guid parentId);
    Task<OrganizationalUnitDto?> GetByIdAsync(Guid id);
    Task<OrganizationalUnitDto> CreateAsync(CreateOrganizationalUnitDto input, string? username);
    Task<OrganizationalUnitDto?> UpdateAsync(Guid id, UpdateOrganizationalUnitDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}