using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services;

public class DownloadAuditService : IDownloadAuditService
{
    private readonly IDownloadAuditRepository _repository;
    private readonly AuditService _auditService;

    public DownloadAuditService(
        IDownloadAuditRepository repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<List<DownloadAuditDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<DownloadAuditDto>> GetByDocumentItemIdAsync(Guid documentItemId)
    {
        var items = await _repository.GetByDocumentItemIdAsync(documentItemId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<DownloadAuditDto>> GetByDocumentFileIdAsync(Guid documentFileId)
    {
        var items = await _repository.GetByDocumentFileIdAsync(documentFileId);
        return items.Select(MapToDto).ToList();
    }

    public async Task RegisterAsync(Guid documentItemId, Guid documentFileId, string? username)
    {
        var entity = new DownloadAudit
        {
            DocumentItemId = documentItemId,
            DocumentFileId = documentFileId,
            DownloadedAt = DateTime.UtcNow,
            DownloadedBy = username
        };

        await _repository.AddAsync(entity);

        _auditService.LogAction(
            "DownloadFile",
            username ?? "Unknown",
            $"Downloaded file for DocumentItem ID: {documentItemId}");
    }

    private static DownloadAuditDto MapToDto(DownloadAudit x)
    {
        return new DownloadAuditDto
        {
            Id = x.Id,
            DocumentItemId = x.DocumentItemId,
            DocumentTitle = x.DocumentItem?.Title ?? "Desconocido",
            DocumentFileId = x.DocumentFileId,
            FileName = x.DocumentFile?.OriginalFileName ?? "Desconocido",
            DownloadedAt = x.DownloadedAt,
            DownloadedBy = x.DownloadedBy
        };
    }
}