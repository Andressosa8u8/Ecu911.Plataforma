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
    private readonly IDownloadAuditService _downloadAuditService;

    public DocumentItemsController(
        IDocumentItemService service,
        IDocumentFileService documentFileService,
        IDownloadAuditService downloadAuditService)
    {
        _service = service;
        _documentFileService = documentFileService;
        _downloadAuditService = downloadAuditService;
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? title,
        [FromQuery] Guid? documentTypeId,
        [FromQuery] Guid? repositoryNodeId,
        [FromQuery] DateTime? createdFrom,
        [FromQuery] DateTime? createdTo,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var isAdmin = UserContextHelper.IsAdmin(User);
        var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

        var filter = new DocumentItemFilterDto
        {
            Title = title,
            DocumentTypeId = documentTypeId,
            RepositoryNodeId = repositoryNodeId,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo
        };

        var result = await _service.GetAllAsync(filter, pageIndex, pageSize, isAdmin, organizationalUnitId);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _service.GetByIdAsync(id, isAdmin, organizationalUnitId);

            if (result == null)
                return NotFound(new { message = "Documento no encontrado." });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentItemDto input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input.Description))
            {
                return BadRequest(new { message = "La descripción es obligatoria." });
            }

            var username = UserContextHelper.GetUsername(User);
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _service.CreateAsync(input, username, isAdmin, organizationalUnitId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentItemDto input)
    {
        try
        {
            var username = UserContextHelper.GetUsername(User);
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _service.UpdateAsync(id, input, username, isAdmin, organizationalUnitId);

            if (result == null)
                return NotFound(new { message = "Documento no encontrado." });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var username = UserContextHelper.GetUsername(User);
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var deleted = await _service.DeleteAsync(id, username, isAdmin, organizationalUnitId);

            if (!deleted)
                return NotFound(new { message = "Documento no encontrado." });

            return Ok(new { message = "Documento marcado como eliminado correctamente." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost("{id:guid}/file")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            var username = UserContextHelper.GetUsername(User);
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _documentFileService.UploadAsync(id, file, username, isAdmin, organizationalUnitId, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> GetFileMetadata(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _documentFileService.GetMetadataAsync(id, isAdmin, organizationalUnitId, cancellationToken);

            if (result == null)
                return NotFound(new { message = "El archivo del documento no fue encontrado." });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}/file/download")]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var result = await _documentFileService.DownloadAsync(id, isAdmin, organizationalUnitId, cancellationToken);

            if (result == null)
                return NotFound(new { message = "El archivo del documento no fue encontrado." });

            var username = UserContextHelper.GetUsername(User);
            await _downloadAuditService.RegisterAsync(id, result.DocumentFileId, username);

            return PhysicalFile(result.AbsolutePath, result.ContentType, result.FileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpDelete("{id:guid}/file")]
    public async Task<IActionResult> DeleteFile(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var username = UserContextHelper.GetUsername(User);
            var isAdmin = UserContextHelper.IsAdmin(User);
            var organizationalUnitId = UserContextHelper.GetOrganizationalUnitId(User);

            var deleted = await _documentFileService.DeleteAsync(id, username, isAdmin, organizationalUnitId, cancellationToken);

            if (!deleted)
                return NotFound(new { message = "El archivo del documento no fue encontrado." });

            return Ok(new { message = "Archivo eliminado correctamente." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}