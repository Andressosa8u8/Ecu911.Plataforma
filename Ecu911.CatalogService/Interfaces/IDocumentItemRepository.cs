using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentItemRepository
{
    Task<List<DocumentItem>> GetAllAsync();
    Task<DocumentItem?> GetByIdAsync(Guid id);
    Task<DocumentItem> AddAsync(DocumentItem item);
    Task<DocumentItem?> UpdateAsync(
        Guid id,
        string title,
        string description,
        Guid documentTypeId,
        Guid? repositoryNodeId,
        string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
    Task<bool> ExistsActiveByDocumentTypeIdAsync(Guid documentTypeId);
}