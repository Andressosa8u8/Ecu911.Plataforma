namespace Ecu911.CatalogService.Models;

public class DocumentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }

    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = default!;
}