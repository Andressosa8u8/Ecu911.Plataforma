namespace Ecu911.CatalogService.DTOs;

public class DocumentFileDownloadDto
{
    public Guid DocumentFileId { get; set; }
    public string AbsolutePath { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
}