namespace Ecu911.CatalogService.DTOs;

public class CreateRepositoryNodeDto
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
    public int DisplayOrder { get; set; } = 0;
}