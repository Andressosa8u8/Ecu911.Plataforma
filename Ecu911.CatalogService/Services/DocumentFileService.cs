using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Ecu911.CatalogService.Services.FileStorage;
using Ecu911.CatalogService.Configuration;
using Microsoft.Extensions.Options;

namespace Ecu911.CatalogService.Services;

public class DocumentFileService : IDocumentFileService
{
    private readonly IDocumentItemRepository _documentItemRepository;
    private readonly IDocumentFileRepository _documentFileRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly AuditService _auditService;
    private readonly FileStorageOptions _options;

    public DocumentFileService(
        IDocumentItemRepository documentItemRepository,
        IDocumentFileRepository documentFileRepository,
        IFileStorageService fileStorageService,
        AuditService auditService,
        IOptions<FileStorageOptions> options)
    {
        _documentItemRepository = documentItemRepository;
        _documentFileRepository = documentFileRepository;
        _fileStorageService = fileStorageService;
        _auditService = auditService;
        _options = options.Value;
    }

    public async Task<DocumentFileDto> UploadAsync(Guid documentItemId, IFormFile file, string? username, CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            throw new ArgumentException("El documento no existe o está eliminado.");
        }

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Debe seleccionar un archivo válido.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_options.AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("La extensión del archivo no está permitida.");
        }

        var maxBytes = _options.MaxFileSizeMB * 1024 * 1024;

        if (file.Length > maxBytes)
        {
            throw new ArgumentException($"El archivo supera el tamaño máximo permitido de {_options.MaxFileSizeMB} MB.");
        }

        var existingFile = await _documentFileRepository.GetByDocumentItemIdAsync(documentItemId);

        if (existingFile != null)
        {
            await _fileStorageService.DeleteAsync(existingFile.RelativePath, cancellationToken);

            existingFile.IsDeleted = true;
            existingFile.DeletedAt = DateTime.UtcNow;
            existingFile.DeletedBy = username;

            await _documentFileRepository.UpdateAsync(existingFile);
        }

        var (storedFileName, storagePath) = await _fileStorageService.SaveAsync(file, cancellationToken);

        var entity = new DocumentFile
        {
            DocumentItemId = documentItemId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            RelativePath = storagePath,
            ContentType = file.ContentType,
            Extension = extension,
            SizeInBytes = file.Length,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = username
        };

        var created = await _documentFileRepository.AddAsync(entity);

        _auditService.LogAction("Upload", username ?? "Unknown", $"Uploaded file for DocumentItem ID: {documentItemId}");

        return new DocumentFileDto
        {
            Id = created.Id,
            DocumentItemId = created.DocumentItemId,
            OriginalFileName = created.OriginalFileName,
            ContentType = created.ContentType,
            Extension = created.Extension,
            SizeInBytes = created.SizeInBytes,
            UploadedAt = created.UploadedAt,
            UploadedBy = created.UploadedBy
        };
    }
}