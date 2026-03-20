namespace Ecu911.CatalogService.DTOs;

public class UpdateNodePermissionDto
{
    public bool CanView { get; set; }
    public bool CanUpload { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
}