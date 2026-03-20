namespace Ecu911.CatalogService.DTOs;

public class DownloadAuditDto
{
    public Guid Id { get; set; }
    public Guid DocumentItemId { get; set; }
    public string DocumentTitle { get; set; } = default!;
    public Guid DocumentFileId { get; set; }
    public string FileName { get; set; } = default!;
    public DateTime DownloadedAt { get; set; }
    public string? DownloadedBy { get; set; }
}