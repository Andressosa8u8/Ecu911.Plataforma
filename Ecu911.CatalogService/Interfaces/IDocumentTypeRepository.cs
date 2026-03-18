using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync();
    Task<DocumentType?> GetByIdAsync(Guid id);
    Task<DocumentType> AddAsync(DocumentType item);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, Guid excludeId);
    Task<DocumentType?> UpdateAsync(Guid id, string name, string description);
    Task<bool> DeactivateAsync(Guid id);
}