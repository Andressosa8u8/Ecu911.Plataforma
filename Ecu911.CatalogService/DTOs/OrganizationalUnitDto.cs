namespace Ecu911.CatalogService.DTOs;

public class OrganizationalUnitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime CreatedAt { get; set; }
}