using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Helpers;
using Ecu911.CatalogService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.CatalogService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeService _service;

    public DocumentTypesController(IDocumentTypeService service)
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

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentTypeDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.CreateAsync(input, username);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentTypeDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.UpdateAsync(id, input, username);

        if (result == null)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(new { message = "Tipo de documento desactivado correctamente." });
    }
}