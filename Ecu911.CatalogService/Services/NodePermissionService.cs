using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class NodePermissionService : INodePermissionService
{
    private readonly INodePermissionRepository _repository;
    private readonly IRepositoryNodeRepository _repositoryNodeRepository;
    private readonly IOrganizationalUnitRepository _organizationalUnitRepository;
    private readonly AuditService _auditService;

    public NodePermissionService(
        INodePermissionRepository repository,
        IRepositoryNodeRepository repositoryNodeRepository,
        IOrganizationalUnitRepository organizationalUnitRepository,
        AuditService auditService)
    {
        _repository = repository;
        _repositoryNodeRepository = repositoryNodeRepository;
        _organizationalUnitRepository = organizationalUnitRepository;
        _auditService = auditService;
    }

    public async Task<List<NodePermissionDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<NodePermissionDto>> GetByNodeIdAsync(Guid repositoryNodeId)
    {
        var items = await _repository.GetByNodeIdAsync(repositoryNodeId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<NodePermissionDto>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId)
    {
        var items = await _repository.GetByOrganizationalUnitIdAsync(organizationalUnitId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<NodePermissionDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? null : MapToDto(item);
    }

    public async Task<NodePermissionDto> CreateAsync(CreateNodePermissionDto input, string? username)
    {
        var nodeExists = await _repositoryNodeRepository.ExistsAsync(input.RepositoryNodeId);
        if (!nodeExists)
        {
            throw new ArgumentException("El nodo del repositorio no existe o está eliminado.");
        }

        var organizationalUnitExists = await _organizationalUnitRepository.ExistsAsync(input.OrganizationalUnitId);
        if (!organizationalUnitExists)
        {
            throw new ArgumentException("La unidad organizacional no existe o está eliminada.");
        }

        var duplicated = await _repository.ExistsAsync(input.RepositoryNodeId, input.OrganizationalUnitId);
        if (duplicated)
        {
            throw new ArgumentException("Ya existe un permiso activo para esa unidad organizacional en ese nodo.");
        }

        var entity = new NodePermission
        {
            RepositoryNodeId = input.RepositoryNodeId,
            OrganizationalUnitId = input.OrganizationalUnitId,
            CanView = input.CanView,
            CanUpload = input.CanUpload,
            CanDownload = input.CanDownload,
            CanDelete = input.CanDelete,
            CanManage = input.CanManage,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = username
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction(
            "Create",
            username ?? "Unknown",
            $"Created NodePermission for RepositoryNode ID: {input.RepositoryNodeId} and OrganizationalUnit ID: {input.OrganizationalUnitId}");

        return MapToDto(created);
    }

    public async Task<NodePermissionDto?> UpdateAsync(Guid id, UpdateNodePermissionDto input, string? username)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.CanView = input.CanView;
        existing.CanUpload = input.CanUpload;
        existing.CanDownload = input.CanDownload;
        existing.CanDelete = input.CanDelete;
        existing.CanManage = input.CanManage;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = username;

        var updated = await _repository.UpdateAsync(existing);

        _auditService.LogAction(
            "Update",
            username ?? "Unknown",
            $"Updated NodePermission with ID: {id}");

        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var deleted = await _repository.DeleteAsync(id, username);

        if (deleted)
        {
            _auditService.LogAction(
                "Delete",
                username ?? "Unknown",
                $"Deleted NodePermission with ID: {id}");
        }

        return deleted;
    }

    private static NodePermissionDto MapToDto(NodePermission x)
    {
        return new NodePermissionDto
        {
            Id = x.Id,
            RepositoryNodeId = x.RepositoryNodeId,
            RepositoryNodeName = x.RepositoryNode?.Name ?? "Desconocido",
            OrganizationalUnitId = x.OrganizationalUnitId,
            OrganizationalUnitName = x.OrganizationalUnit?.Name ?? "Desconocido",
            CanView = x.CanView,
            CanUpload = x.CanUpload,
            CanDownload = x.CanDownload,
            CanDelete = x.CanDelete,
            CanManage = x.CanManage,
            CreatedAt = x.CreatedAt
        };
    }
}