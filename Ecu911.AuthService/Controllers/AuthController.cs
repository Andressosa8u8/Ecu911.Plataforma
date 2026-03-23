using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var result = await _authService.LoginAsync(input);

        if (result == null)
            return Unauthorized("Credenciales inválidas.");

        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("No se pudo identificar al usuario autenticado.");

        var result = await _authService.GetCurrentUserAsync(userId);

        if (result == null)
            return NotFound("Usuario no encontrado.");

        result.CurrentSystem = User.FindFirstValue("system_code");

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _authService.GetUsersAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto input)
    {
        var result = await _authService.CreateUserAsync(input);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto input)
    {
        var result = await _authService.CreateRoleAsync(input);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("users/{userId:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleDto input)
    {
        await _authService.AssignRoleAsync(userId, input.RoleId);
        return Ok();
    }
}