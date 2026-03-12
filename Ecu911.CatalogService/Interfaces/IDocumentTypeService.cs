using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentTypeService
{
    Task<List<DocumentTypeDto>> GetAllAsync();
    Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto input);
}