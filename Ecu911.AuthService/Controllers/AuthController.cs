using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto input)
    {
        try
        {
            var result = await _authService.CreateUserAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _authService.GetUsersAsync();
        return Ok(result);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto input)
    {
        try
        {
            var result = await _authService.CreateRoleAsync(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("users/{userId:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleDto input)
    {
        try
        {
            await _authService.AssignRoleAsync(userId, input.RoleId);
            return Ok(new { message = "Rol asignado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var result = await _authService.LoginAsync(input);

        if (result == null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        return Ok(result);
    }
}