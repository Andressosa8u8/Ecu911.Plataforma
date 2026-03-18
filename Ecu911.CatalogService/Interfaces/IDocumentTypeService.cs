using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentTypeService
{
    Task<List<DocumentTypeDto>> GetAllAsync();
    Task<DocumentTypeDto?> GetByIdAsync(Guid id);
    Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto input, string? username);
    Task<DocumentTypeDto?> UpdateAsync(Guid id, UpdateDocumentTypeDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}