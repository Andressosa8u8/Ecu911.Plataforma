using Ecu911.CatalogService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.CatalogService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DownloadAuditsController : ControllerBase
{
    private readonly IDownloadAuditService _service;

    public DownloadAuditsController(IDownloadAuditService service)
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
    [HttpGet("by-document/{documentItemId:guid}")]
    public async Task<IActionResult> GetByDocument(Guid documentItemId)
    {
        var result = await _service.GetByDocumentItemIdAsync(documentItemId);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("by-file/{documentFileId:guid}")]
    public async Task<IActionResult> GetByFile(Guid documentFileId)
    {
        var result = await _service.GetByDocumentFileIdAsync(documentFileId);
        return Ok(result);
    }
}