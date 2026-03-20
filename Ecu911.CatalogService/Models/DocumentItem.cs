using System.ComponentModel.DataAnnotations;

namespace Ecu911.CatalogService.Models;

public class DocumentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = default!;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public Guid DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }

    public Guid? RepositoryNodeId { get; set; }
    public RepositoryNode? RepositoryNode { get; set; }

    public DocumentFile? DocumentFile { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public ICollection<DownloadAudit> DownloadAudits { get; set; } = new List<DownloadAudit>();
}