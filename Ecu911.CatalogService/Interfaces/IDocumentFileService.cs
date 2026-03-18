using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentFileService
{
    Task<DocumentFileDto> UploadAsync(Guid documentItemId, IFormFile file, string? username, CancellationToken cancellationToken = default);
}