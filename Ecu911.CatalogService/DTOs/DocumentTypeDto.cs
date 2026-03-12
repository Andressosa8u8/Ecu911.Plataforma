namespace Ecu911.CatalogService.DTOs;

public class DocumentTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}