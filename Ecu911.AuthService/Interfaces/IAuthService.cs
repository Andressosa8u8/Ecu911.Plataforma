using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IAuthService
{
    Task<UserDto> CreateUserAsync(CreateUserDto input);
    Task<Role> CreateRoleAsync(CreateRoleDto input);
    Task AssignRoleAsync(Guid userId, Guid roleId);
    Task<List<UserDto>> GetUsersAsync();
    Task<LoginResponseDto?> LoginAsync(LoginDto input);
}