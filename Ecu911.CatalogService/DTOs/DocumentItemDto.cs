namespace Ecu911.CatalogService.DTOs;

public class DocumentItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Guid DocumentTypeId { get; set; }
    public string DocumentTypeName { get; set; } = default!;

    public Guid? RepositoryNodeId { get; set; }
    public string? RepositoryNodeName { get; set; }
}