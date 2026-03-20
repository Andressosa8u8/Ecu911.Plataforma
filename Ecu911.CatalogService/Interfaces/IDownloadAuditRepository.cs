using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDownloadAuditRepository
{
    Task<List<DownloadAudit>> GetAllAsync();
    Task<List<DownloadAudit>> GetByDocumentItemIdAsync(Guid documentItemId);
    Task<List<DownloadAudit>> GetByDocumentFileIdAsync(Guid documentFileId);
    Task<DownloadAudit> AddAsync(DownloadAudit entity);
}