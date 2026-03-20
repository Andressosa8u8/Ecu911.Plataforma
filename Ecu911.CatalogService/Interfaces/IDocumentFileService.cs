using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentFileService
{
    Task<DocumentFileDto> UploadAsync(Guid documentItemId, IFormFile file, string? username, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<DocumentFileDto?> GetMetadataAsync(Guid documentItemId, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<DocumentFileDownloadDto?> DownloadAsync(Guid documentItemId, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid documentItemId, string? username, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
}