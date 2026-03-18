using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Helpers;
using Ecu911.CatalogService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.CatalogService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentItemsController : ControllerBase
{
    private readonly IDocumentItemService _service;
    private readonly IDocumentFileService _documentFileService;

    public DocumentItemsController(IDocumentItemService service, IDocumentFileService documentFileService)
    {
        _service = service;
        _documentFileService = documentFileService;
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
    {
        var result = await _service.GetAllAsync(pageIndex, pageSize);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentItemDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Description))
        {
            return BadRequest(new { message = "La descripción es obligatoria." });
        }

        var username = UserContextHelper.GetUsername(User);
        var result = await _service.CreateAsync(input, username);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentItemDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.UpdateAsync(id, input, username);

        if (result == null)
            return NotFound(new { message = "Documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound(new { message = "Documento no encontrado." });

        return Ok(new { message = "Documento marcado como eliminado correctamente." });
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost("{id:guid}/file")]
    public async Task<IActionResult> UploadFile(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _documentFileService.UploadAsync(id, file, username, cancellationToken);
        return Ok(result);
    }
}