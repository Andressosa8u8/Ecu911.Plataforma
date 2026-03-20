using Ecu911.CatalogService.Configuration;
using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Ecu911.CatalogService.Services.FileStorage;
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

        // Busca cualquier registro previo, incluso si está eliminado
        var existingFile = await _documentFileRepository.GetAnyByDocumentItemIdAsync(documentItemId);

        // Si hay un archivo activo previo, se elimina físicamente antes de reemplazarlo
        if (existingFile != null && !existingFile.IsDeleted && !string.IsNullOrWhiteSpace(existingFile.RelativePath))
        {
            await _fileStorageService.DeleteAsync(existingFile.RelativePath, cancellationToken);
        }

        var (storedFileName, storagePath) = await _fileStorageService.SaveAsync(file, cancellationToken);

        if (existingFile != null)
        {
            existingFile.OriginalFileName = file.FileName;
            existingFile.StoredFileName = storedFileName;
            existingFile.RelativePath = storagePath;
            existingFile.ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;
            existingFile.Extension = extension;
            existingFile.SizeInBytes = file.Length;
            existingFile.UploadedAt = DateTime.UtcNow;
            existingFile.UploadedBy = username;
            existingFile.IsDeleted = false;
            existingFile.DeletedAt = null;
            existingFile.DeletedBy = null;

            var updated = await _documentFileRepository.UpdateAsync(existingFile);

            _auditService.LogAction("UploadFile", username ?? "Unknown", $"Uploaded file for DocumentItem ID: {documentItemId}");

            return new DocumentFileDto
            {
                Id = updated.Id,
                DocumentItemId = updated.DocumentItemId,
                OriginalFileName = updated.OriginalFileName,
                ContentType = updated.ContentType,
                Extension = updated.Extension,
                SizeInBytes = updated.SizeInBytes,
                UploadedAt = updated.UploadedAt,
                UploadedBy = updated.UploadedBy
            };
        }

        var entity = new DocumentFile
        {
            DocumentItemId = documentItemId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            RelativePath = storagePath,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
            Extension = extension,
            SizeInBytes = file.Length,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = username
        };

        var created = await _documentFileRepository.AddAsync(entity);

        _auditService.LogAction("UploadFile", username ?? "Unknown", $"Uploaded file for DocumentItem ID: {documentItemId}");

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

    public async Task<DocumentFileDto?> GetMetadataAsync(
        Guid documentItemId,
        CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return null;
        }

        var file = await _documentFileRepository.GetByDocumentItemIdAsync(documentItemId);

        if (file == null)
        {
            return null;
        }

        return MapToDto(file);
    }

    public async Task<DocumentFileDownloadDto?> DownloadAsync(
    Guid documentItemId,
    CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return null;
        }

        var file = await _documentFileRepository.GetByDocumentItemIdAsync(documentItemId);

        if (file == null)
        {
            return null;
        }

        var absolutePath = _fileStorageService.GetAbsolutePath(file.RelativePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException("El archivo físico no existe en la ruta configurada.");
        }

        return new DocumentFileDownloadDto
        {
            DocumentFileId = file.Id,
            AbsolutePath = absolutePath,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
            FileName = file.OriginalFileName
        };
    }

    public async Task<bool> DeleteAsync(
        Guid documentItemId,
        string? username,
        CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return false;
        }

        var existingFile = await _documentFileRepository.GetByDocumentItemIdAsync(documentItemId);

        if (existingFile == null)
        {
            return false;
        }

        await _fileStorageService.DeleteAsync(existingFile.RelativePath, cancellationToken);

        existingFile.IsDeleted = true;
        existingFile.DeletedAt = DateTime.UtcNow;
        existingFile.DeletedBy = username;

        await _documentFileRepository.UpdateAsync(existingFile);

        _auditService.LogAction(
            "DeleteFile",
            username ?? "Unknown",
            $"Deleted file for DocumentItem ID: {documentItemId}");

        return true;
    }

    private static DocumentFileDto MapToDto(DocumentFile entity)
    {
        return new DocumentFileDto
        {
            Id = entity.Id,
            DocumentItemId = entity.DocumentItemId,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            Extension = entity.Extension,
            SizeInBytes = entity.SizeInBytes,
            UploadedAt = entity.UploadedAt,
            UploadedBy = entity.UploadedBy
        };
    }
}