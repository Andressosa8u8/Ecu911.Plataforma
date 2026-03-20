using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class RepositoryNodeService : IRepositoryNodeService
{
    private readonly IRepositoryNodeRepository _repository;
    private readonly IOrganizationalUnitRepository _organizationalUnitRepository;
    private readonly AuditService _auditService;

    public RepositoryNodeService(
        IRepositoryNodeRepository repository,
        IOrganizationalUnitRepository organizationalUnitRepository,
        AuditService auditService)
    {
        _repository = repository;
        _organizationalUnitRepository = organizationalUnitRepository;
        _auditService = auditService;
    }

    public async Task<List<RepositoryNodeDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<RepositoryNodeDto>> GetRootsAsync()
    {
        var items = await _repository.GetRootsAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<RepositoryNodeDto>> GetChildrenAsync(Guid parentId)
    {
        var items = await _repository.GetChildrenAsync(parentId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<RepositoryNodeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? null : MapToDto(item);
    }

    public async Task<RepositoryNodeDto> CreateAsync(CreateRepositoryNodeDto input, string? username)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new ArgumentException("El nombre del nodo es obligatorio.");
        }

        if (input.ParentId.HasValue)
        {
            var parentExists = await _repository.ExistsAsync(input.ParentId.Value);

            if (!parentExists)
            {
                throw new ArgumentException("El nodo padre no existe o está eliminado.");
            }
        }

        if (input.OrganizationalUnitId.HasValue)
        {
            var organizationalUnitExists = await _organizationalUnitRepository.ExistsAsync(input.OrganizationalUnitId.Value);

            if (!organizationalUnitExists)
            {
                throw new ArgumentException("La unidad organizacional no existe o está eliminada.");
            }
        }

        var duplicated = await _repository.ExistsActiveByNameAsync(input.Name.Trim(), input.ParentId);

        if (duplicated)
        {
            throw new ArgumentException("Ya existe un nodo activo con ese nombre en el mismo nivel.");
        }

        var entity = new RepositoryNode
        {
            Name = input.Name.Trim(),
            Code = string.IsNullOrWhiteSpace(input.Code) ? null : input.Code.Trim(),
            Description = input.Description?.Trim() ?? string.Empty,
            ParentId = input.ParentId,
            Module = NormalizeModule(input.Module),
            OrganizationalUnitId = input.OrganizationalUnitId,
            DisplayOrder = input.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = username
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction(
            "Create",
            username ?? "Unknown",
            $"Created RepositoryNode with name: {created.Name}");

        return MapToDto(created);
    }

    public async Task<RepositoryNodeDto?> UpdateAsync(Guid id, UpdateRepositoryNodeDto input, string? username)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new ArgumentException("El nombre del nodo es obligatorio.");
        }

        if (input.ParentId == id)
        {
            throw new ArgumentException("Un nodo no puede ser padre de sí mismo.");
        }

        if (input.ParentId.HasValue)
        {
            var parentExists = await _repository.ExistsAsync(input.ParentId.Value);

            if (!parentExists)
            {
                throw new ArgumentException("El nodo padre no existe o está eliminado.");
            }
        }

        if (input.OrganizationalUnitId.HasValue)
        {
            var organizationalUnitExists = await _organizationalUnitRepository.ExistsAsync(input.OrganizationalUnitId.Value);

            if (!organizationalUnitExists)
            {
                throw new ArgumentException("La unidad organizacional no existe o está eliminada.");
            }
        }

        var duplicated = await _repository.ExistsActiveByNameAsync(input.Name.Trim(), input.ParentId, id);

        if (duplicated)
        {
            throw new ArgumentException("Ya existe un nodo activo con ese nombre en el mismo nivel.");
        }

        existing.Name = input.Name.Trim();
        existing.Code = string.IsNullOrWhiteSpace(input.Code) ? null : input.Code.Trim();
        existing.Description = input.Description?.Trim() ?? string.Empty;
        existing.ParentId = input.ParentId;
        existing.Module = NormalizeModule(input.Module);
        existing.OrganizationalUnitId = input.OrganizationalUnitId;
        existing.DisplayOrder = input.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = username;

        var updated = await _repository.UpdateAsync(existing);

        _auditService.LogAction(
            "Update",
            username ?? "Unknown",
            $"Updated RepositoryNode with ID: {id}");

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
            throw new ArgumentException("No se puede eliminar el nodo porque tiene nodos hijos activos.");
        }

        var deleted = await _repository.DeleteAsync(id, username);

        if (deleted)
        {
            _auditService.LogAction(
                "Delete",
                username ?? "Unknown",
                $"Deleted RepositoryNode with ID: {id}");
        }

        return deleted;
    }

    private static RepositoryNodeDto MapToDto(RepositoryNode x)
    {
        return new RepositoryNodeDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            ParentId = x.ParentId,
            Module = x.Module,
            ParentName = x.Parent?.Name,
            OrganizationalUnitId = x.OrganizationalUnitId,
            OrganizationalUnitName = x.OrganizationalUnit?.Name,
            DisplayOrder = x.DisplayOrder,
            CreatedAt = x.CreatedAt
        };
    }

    private static string NormalizeModule(string? module)
{
    if (string.IsNullOrWhiteSpace(module))
        return "REPOSITORIO";

    var normalized = module.Trim().ToUpper();

    if (normalized != "REPOSITORIO" && normalized != "BIBLIOTECA")
    {
        throw new ArgumentException("El módulo debe ser REPOSITORIO o BIBLIOTECA.");
    }

    return normalized;
}
}