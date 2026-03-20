using System.ComponentModel.DataAnnotations;

namespace Ecu911.CatalogService.Models;

public class RepositoryNode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }
    public RepositoryNode? Parent { get; set; }

    public ICollection<RepositoryNode> Children { get; set; } = new List<RepositoryNode>();

    public Guid? OrganizationalUnitId { get; set; }
    public OrganizationalUnit? OrganizationalUnit { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}