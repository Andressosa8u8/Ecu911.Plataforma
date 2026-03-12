using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync();
    Task<DocumentType?> GetByIdAsync(Guid id);
    Task<DocumentType> AddAsync(DocumentType entity);
}