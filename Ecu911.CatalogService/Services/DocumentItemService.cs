using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class DocumentItemService : IDocumentItemService
{
    private readonly IDocumentItemRepository _repository;

    public DocumentItemService(IDocumentItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DocumentItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<DocumentItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? null : MapToDto(item);
    }

    public async Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username)
    {
        // Verificación de campos obligatorios
        if (string.IsNullOrWhiteSpace(input.Description))  // Si Description está vacío o es solo espacio
        {
            throw new ArgumentException("La descripción es obligatoria y no puede estar vacía.");
        }

        if (input.DocumentTypeId == Guid.Empty)  // Verifica que el DocumentTypeId no sea vacío
        {
            throw new ArgumentException("El DocumentTypeId es obligatorio.");
        }

        var entity = new DocumentItem
        {
            Title = input.Title,
            Description = input.Description,  // Asignamos Description
            DocumentTypeId = input.DocumentTypeId,  // Asignamos DocumentTypeId
            CreatedBy = username,  // Usuario que realiza la creación
            CreatedAt = DateTime.UtcNow  // Asignamos la fecha de creación
        };

        // Guardar en la base de datos
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username)
    {
        var updated = await _repository.UpdateAsync(id, input.Title, input.Description, input.DocumentTypeId, username);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        return await _repository.DeleteAsync(id, username);
    }

    private static DocumentItemDto MapToDto(DocumentItem x)
    {
        return new DocumentItemDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
            DocumentTypeId = x.DocumentTypeId,
            DocumentTypeName = x.DocumentType?.Name ?? "Desconocido"
        };
    }
}