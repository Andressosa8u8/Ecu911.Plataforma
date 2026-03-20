namespace Ecu911.CatalogService.DTOs;

public class NodePermissionDto
{
    public Guid Id { get; set; }
    public Guid RepositoryNodeId { get; set; }
    public string RepositoryNodeName { get; set; } = default!;
    public Guid OrganizationalUnitId { get; set; }
    public string OrganizationalUnitName { get; set; } = default!;
    public bool CanView { get; set; }
    public bool CanUpload { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
    public DateTime CreatedAt { get; set; }
}