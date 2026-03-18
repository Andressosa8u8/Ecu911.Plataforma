namespace Ecu911.CatalogService.DTOs;

public class DocumentFileDto
{
    public Guid Id { get; set; }
    public Guid DocumentItemId { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long SizeInBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
}