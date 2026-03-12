using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentItemService
{
    Task<List<DocumentItemDto>> GetAllAsync();
    Task<DocumentItemDto?> GetByIdAsync(Guid id);
    Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username);
    Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}