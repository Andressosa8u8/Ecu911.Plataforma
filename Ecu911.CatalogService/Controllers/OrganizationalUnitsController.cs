using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Helpers;
using Ecu911.CatalogService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.CatalogService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrganizationalUnitsController : ControllerBase
{
    private readonly IOrganizationalUnitService _service;

    public OrganizationalUnitsController(IOrganizationalUnitService service)
    {
        _service = service;
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("roots")]
    public async Task<IActionResult> GetRoots()
    {
        var result = await _service.GetRootsAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Unidad organizacional no encontrada." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildren(Guid id)
    {
        var result = await _service.GetChildrenAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationalUnitDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.CreateAsync(input, username);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationalUnitDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.UpdateAsync(id, input, username);

        if (result == null)
            return NotFound(new { message = "Unidad organizacional no encontrada." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound(new { message = "Unidad organizacional no encontrada." });

        return Ok(new { message = "Unidad organizacional eliminada correctamente." });
    }
}