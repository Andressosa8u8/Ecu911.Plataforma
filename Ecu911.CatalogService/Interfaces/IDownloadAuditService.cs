using Ecu911.CatalogService.DTOs;

namespace Ecu911.CatalogService.Interfaces;

public interface IDownloadAuditService
{
    Task<List<DownloadAuditDto>> GetAllAsync();
    Task<List<DownloadAuditDto>> GetByDocumentItemIdAsync(Guid documentItemId);
    Task<List<DownloadAuditDto>> GetByDocumentFileIdAsync(Guid documentFileId);
    Task RegisterAsync(Guid documentItemId, Guid documentFileId, string? username);
}