namespace Ecu911.CatalogService.DTOs;

public class DocumentItemFilterDto
{
    public string? Title { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public Guid? RepositoryNodeId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}