using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.AuthService.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/[controller]")]
public class SystemModulesController : ControllerBase
{
    private readonly IAuthService _authService;

    public SystemModulesController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _authService.GetSystemModulesAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSystemModuleDto input)
    {
        var result = await _authService.CreateSystemModuleAsync(input);
        return Ok(result);
    }

    [HttpPost("assign-user-role")]
    public async Task<IActionResult> AssignUserRole([FromBody] AssignUserSystemRoleDto input)
    {
        await _authService.AssignUserSystemRoleAsync(input);
        return Ok(new { message = "Rol asignado al usuario para el sistema correctamente." });
    }
}