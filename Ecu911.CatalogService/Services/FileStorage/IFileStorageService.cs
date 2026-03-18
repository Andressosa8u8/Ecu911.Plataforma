namespace Ecu911.CatalogService.Services.FileStorage;

public interface IFileStorageService
{
    Task<(string StoredFileName, string RelativePath)> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
    string GetAbsolutePath(string relativePath);
}