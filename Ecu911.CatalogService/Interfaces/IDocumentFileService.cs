using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentFileService
{
    Task<DocumentFileDto> UploadAsync(
        Guid documentItemId,
        IFormFile file,
        string? username,
        CancellationToken cancellationToken = default);

    Task<DocumentFileDto?> GetMetadataAsync(
        Guid documentItemId,
        CancellationToken cancellationToken = default);

    Task<DocumentFileDownloadDto?> DownloadAsync(
        Guid documentItemId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid documentItemId,
        string? username,
        CancellationToken cancellationToken = default);
}