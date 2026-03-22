using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.AuthService.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/[controller]")]
public class AuthorizationAdminController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthorizationAdminController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _authService.GetPermissionsAsync();
        return Ok(result);
    }

    [HttpPost("permissions")]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto input)
    {
        var result = await _authService.CreatePermissionAsync(input);
        return Ok(result);
    }

    [HttpPost("assign-role-permission")]
    public async Task<IActionResult> AssignRolePermission([FromBody] AssignRolePermissionDto input)
    {
        await _authService.AssignRolePermissionAsync(input);
        return Ok(new { message = "Permiso asignado al rol correctamente." });
    }

    [HttpPost("assign-user-system-scope")]
    public async Task<IActionResult> AssignUserSystemScope([FromBody] AssignUserSystemScopeDto input)
    {
        await _authService.AssignUserSystemScopeAsync(input);
        return Ok(new { message = "Alcance asignado al usuario para el sistema correctamente." });
    }
}