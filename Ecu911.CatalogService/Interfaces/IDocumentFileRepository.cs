using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentFileRepository
{
    Task<DocumentFile?> GetByDocumentItemIdAsync(Guid documentItemId);
    Task<DocumentFile> AddAsync(DocumentFile file);
    Task<DocumentFile?> UpdateAsync(DocumentFile file);
}