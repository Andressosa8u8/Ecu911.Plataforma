namespace Ecu911.CatalogService.Models;

public class DownloadAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DocumentItemId { get; set; }
    public DocumentItem? DocumentItem { get; set; }

    public Guid DocumentFileId { get; set; }
    public DocumentFile? DocumentFile { get; set; }

    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public string? DownloadedBy { get; set; }
}