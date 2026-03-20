namespace Ecu911.CatalogService.Models;

public class DocumentFile
{
    public Guid Id { get; set; }

    public Guid DocumentItemId { get; set; }
    public DocumentItem DocumentItem { get; set; } = default!;

    public string OriginalFileName { get; set; } = default!;
    public string StoredFileName { get; set; } = default!;
    public string RelativePath { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Extension { get; set; } = default!;

    public long SizeInBytes { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UploadedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public ICollection<DownloadAudit> DownloadAudits { get; set; } = new List<DownloadAudit>();
}