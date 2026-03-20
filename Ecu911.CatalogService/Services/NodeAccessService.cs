using Ecu911.CatalogService.Interfaces;

namespace Ecu911.CatalogService.Services;

public class NodeAccessService : INodeAccessService
{
    private readonly INodePermissionRepository _nodePermissionRepository;

    public NodeAccessService(INodePermissionRepository nodePermissionRepository)
    {
        _nodePermissionRepository = nodePermissionRepository;
    }

    public async Task<bool> CanViewNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return true;

        if (!organizationalUnitId.HasValue)
            return false;

        var permission = await _nodePermissionRepository
            .GetByNodeIdAndOrganizationalUnitIdAsync(repositoryNodeId, organizationalUnitId.Value);

        return permission != null && (permission.CanView || permission.CanManage);
    }

    public async Task<bool> CanUploadToNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return true;

        if (!organizationalUnitId.HasValue)
            return false;

        var permission = await _nodePermissionRepository
            .GetByNodeIdAndOrganizationalUnitIdAsync(repositoryNodeId, organizationalUnitId.Value);

        return permission != null && (permission.CanUpload || permission.CanManage);
    }

    public async Task<bool> CanDownloadFromNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return true;

        if (!organizationalUnitId.HasValue)
            return false;

        var permission = await _nodePermissionRepository
            .GetByNodeIdAndOrganizationalUnitIdAsync(repositoryNodeId, organizationalUnitId.Value);

        return permission != null && (permission.CanDownload || permission.CanManage);
    }

    public async Task<bool> CanDeleteFromNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return true;

        if (!organizationalUnitId.HasValue)
            return false;

        var permission = await _nodePermissionRepository
            .GetByNodeIdAndOrganizationalUnitIdAsync(repositoryNodeId, organizationalUnitId.Value);

        return permission != null && (permission.CanDelete || permission.CanManage);
    }

    public async Task<bool> CanManageNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return true;

        if (!organizationalUnitId.HasValue)
            return false;

        var permission = await _nodePermissionRepository
            .GetByNodeIdAndOrganizationalUnitIdAsync(repositoryNodeId, organizationalUnitId.Value);

        return permission != null && permission.CanManage;
    }

    public async Task<List<Guid>> GetReadableNodeIdsAsync(bool isAdmin, Guid? organizationalUnitId)
    {
        if (isAdmin)
            return new List<Guid>();

        if (!organizationalUnitId.HasValue)
            return new List<Guid>();

        return await _nodePermissionRepository
            .GetReadableNodeIdsByOrganizationalUnitIdAsync(organizationalUnitId.Value);
    }
}