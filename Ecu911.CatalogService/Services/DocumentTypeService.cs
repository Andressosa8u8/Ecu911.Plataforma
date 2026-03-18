using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IDocumentItemRepository _documentItemRepository;
    private readonly AuditService _auditService;

    public DocumentTypeService(
        IDocumentTypeRepository repository,
        IDocumentItemRepository documentItemRepository,
        AuditService auditService)
    {
        _repository = repository;
        _documentItemRepository = documentItemRepository;
        _auditService = auditService;
    }

    public async Task<List<DocumentTypeDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return items.Select(x => new DocumentTypeDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<DocumentTypeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);

        if (item == null)
            return null;

        return new DocumentTypeDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }

    public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto input, string? username)
    {
        var duplicateName = await _repository.NameExistsAsync(input.Name);

        if (duplicateName)
        {
            throw new ArgumentException("Ya existe un tipo de documento activo con ese nombre.");
        }

        var entity = new DocumentType
        {
            Name = input.Name,
            Description = input.Description,
            IsActive = true
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction("Create", username ?? "Unknown", $"Created DocumentType with name: {input.Name}");

        return new DocumentTypeDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<DocumentTypeDto?> UpdateAsync(Guid id, UpdateDocumentTypeDto input, string? username)
    {
        var exists = await _repository.ExistsAsync(id);

        if (!exists)
        {
            throw new ArgumentException("El tipo de documento no existe o está inactivo.");
        }

        var duplicateName = await _repository.NameExistsAsync(input.Name, id);

        if (duplicateName)
        {
            throw new ArgumentException("Ya existe un tipo de documento activo con ese nombre.");
        }

        var updated = await _repository.UpdateAsync(id, input.Name, input.Description);

        if (updated != null)
        {
            _auditService.LogAction("Update", username ?? "Unknown", $"Updated DocumentType with ID: {id}");
        }

        return updated == null
            ? null
            : new DocumentTypeDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                IsActive = updated.IsActive,
                CreatedAt = updated.CreatedAt
            };
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var exists = await _repository.ExistsAsync(id);

        if (!exists)
        {
            throw new ArgumentException("El tipo de documento no existe o está inactivo.");
        }

        var isInUse = await _documentItemRepository.ExistsActiveByDocumentTypeIdAsync(id);

        if (isInUse)
        {
            throw new ArgumentException("No se puede desactivar el tipo de documento porque está siendo usado por documentos activos.");
        }

        var deleted = await _repository.DeactivateAsync(id);

        if (deleted)
        {
            _auditService.LogAction("Delete", username ?? "Unknown", $"Deactivated DocumentType with ID: {id}");
        }

        return deleted;
    }
}