using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _repository;

    public DocumentTypeService(IDocumentTypeRepository repository)
    {
        _repository = repository;
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

    public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto input)
    {
        var entity = new DocumentType
        {
            Name = input.Name,
            Description = input.Description,
            IsActive = true
        };

        var created = await _repository.AddAsync(entity);

        return new DocumentTypeDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }
}