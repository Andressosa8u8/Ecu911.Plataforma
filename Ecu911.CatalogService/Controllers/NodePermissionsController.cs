using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Helpers;
using Ecu911.CatalogService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.CatalogService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NodePermissionsController : ControllerBase
{
    private readonly INodePermissionService _service;

    public NodePermissionsController(INodePermissionService service)
    {
        _service = service;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Permiso por nodo no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("by-node/{repositoryNodeId:guid}")]
    public async Task<IActionResult> GetByNode(Guid repositoryNodeId)
    {
        var result = await _service.GetByNodeIdAsync(repositoryNodeId);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("by-organizational-unit/{organizationalUnitId:guid}")]
    public async Task<IActionResult> GetByOrganizationalUnit(Guid organizationalUnitId)
    {
        var result = await _service.GetByOrganizationalUnitIdAsync(organizationalUnitId);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNodePermissionDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.CreateAsync(input, username);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNodePermissionDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.UpdateAsync(id, input, username);

        if (result == null)
            return NotFound(new { message = "Permiso por nodo no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound(new { message = "Permiso por nodo no encontrado." });

        return Ok(new { message = "Permiso por nodo eliminado correctamente." });
    }
}