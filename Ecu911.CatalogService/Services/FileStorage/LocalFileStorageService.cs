using Ecu911.CatalogService.Configuration;
using Microsoft.Extensions.Options;

namespace Ecu911.CatalogService.Services.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(
        IOptions<FileStorageOptions> options,
        IWebHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public async Task<(string StoredFileName, string RelativePath)> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var basePath = _options.BasePath;
        Directory.CreateDirectory(basePath);

        var absolutePath = Path.Combine(basePath, storedFileName);

        await using var stream = new FileStream(absolutePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return (storedFileName, absolutePath);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(relativePath))
        {
            File.Delete(relativePath);
        }

        return Task.CompletedTask;
    }

    public string GetAbsolutePath(string relativePath)
    {
        return relativePath;
    }
}