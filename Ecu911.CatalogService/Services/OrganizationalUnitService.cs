using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Ecu911.CatalogService.Repositories;

namespace Ecu911.CatalogService.Services;

public class OrganizationalUnitService : IOrganizationalUnitService
{
    private readonly IOrganizationalUnitRepository _repository;
    private readonly AuditService _auditService;

    public OrganizationalUnitService(
        IOrganizationalUnitRepository repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<List<OrganizationalUnitDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<OrganizationalUnitDto>> GetRootsAsync()
    {
        var items = await _repository.GetRootsAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<OrganizationalUnitDto>> GetChildrenAsync(Guid parentId)
    {
        var items = await _repository.GetChildrenAsync(parentId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<OrganizationalUnitDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? null : MapToDto(item);
    }

    public async Task<OrganizationalUnitDto> CreateAsync(CreateOrganizationalUnitDto input, string? username)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new ArgumentException("El nombre de la unidad organizacional es obligatorio.");
        }

        if (input.ParentId.HasValue)
        {
            var parentExists = await _repository.ExistsAsync(input.ParentId.Value);

            if (!parentExists)
            {
                throw new ArgumentException("La unidad organizacional padre no existe o está eliminada.");
            }
        }

        var duplicated = await _repository.ExistsActiveByNameAsync(input.Name.Trim(), input.ParentId);

        if (duplicated)
        {
            throw new ArgumentException("Ya existe una unidad organizacional activa con ese nombre en el mismo nivel.");
        }

        var entity = new OrganizationalUnit
        {
            Name = input.Name.Trim(),
            Code = string.IsNullOrWhiteSpace(input.Code) ? null : input.Code.Trim(),
            Description = input.Description?.Trim() ?? string.Empty,
            ParentId = input.ParentId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = username
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction(
            "Create",
            username ?? "Unknown",
            $"Created OrganizationalUnit with name: {created.Name}");

        return MapToDto(created);
    }

    public async Task<OrganizationalUnitDto?> UpdateAsync(Guid id, UpdateOrganizationalUnitDto input, string? username)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new ArgumentException("El nombre de la unidad organizacional es obligatorio.");
        }

        if (input.ParentId == id)
        {
            throw new ArgumentException("Una unidad organizacional no puede ser padre de sí misma.");
        }

        if (input.ParentId.HasValue)
        {
            var parentExists = await _repository.ExistsAsync(input.ParentId.Value);

            if (!parentExists)
            {
                throw new ArgumentException("La unidad organizacional padre no existe o está eliminada.");
            }
        }

        var duplicated = await _repository.ExistsActiveByNameAsync(input.Name.Trim(), input.ParentId, id);

        if (duplicated)
        {
            throw new ArgumentException("Ya existe una unidad organizacional activa con ese nombre en el mismo nivel.");
        }

        existing.Name = input.Name.Trim();
        existing.Code = string.IsNullOrWhiteSpace(input.Code) ? null : input.Code.Trim();
        existing.Description = input.Description?.Trim() ?? string.Empty;
        existing.ParentId = input.ParentId;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = username;

        var updated = await _repository.UpdateAsync(existing);

        _auditService.LogAction(
            "Update",
            username ?? "Unknown",
            $"Updated OrganizationalUnit with ID: {id}");

        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
        {
            return false;
        }

        var hasChildren = await _repository.HasActiveChildrenAsync(id);

        if (hasChildren)
        {
            throw new ArgumentException("No se puede eliminar la unidad organizacional porque tiene unidades hijas activas.");
        }

        var deleted = await _repository.DeleteAsync(id, username);

        if (deleted)
        {
            _auditService.LogAction(
                "Delete",
                username ?? "Unknown",
                $"Deleted OrganizationalUnit with ID: {id}");
        }

        return deleted;
    }

    private static OrganizationalUnitDto MapToDto(OrganizationalUnit x)
    {
        return new OrganizationalUnitDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            ParentId = x.ParentId,
            ParentName = x.Parent?.Name,
            CreatedAt = x.CreatedAt
        };
    }
}