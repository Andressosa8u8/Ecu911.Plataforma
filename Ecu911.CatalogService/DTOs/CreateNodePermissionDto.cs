namespace Ecu911.CatalogService.DTOs;

public class CreateNodePermissionDto
{
    public Guid RepositoryNodeId { get; set; }
    public Guid OrganizationalUnitId { get; set; }
    public bool CanView { get; set; }
    public bool CanUpload { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
}