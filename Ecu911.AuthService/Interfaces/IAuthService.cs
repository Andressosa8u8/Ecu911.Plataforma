using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IAuthService
{
    Task<UserDto> CreateUserAsync(CreateUserDto input);
    Task<Role> CreateRoleAsync(CreateRoleDto input);
    Task AssignRoleAsync(Guid userId, Guid roleId);
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
    Task<LoginResponseDto?> LoginAsync(LoginDto input);
    Task<SystemModuleDto> CreateSystemModuleAsync(CreateSystemModuleDto input);
    Task<List<SystemModuleDto>> GetSystemModulesAsync();
    Task AssignUserSystemRoleAsync(AssignUserSystemRoleDto input);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto input);
    Task<List<PermissionDto>> GetPermissionsAsync();
    Task AssignRolePermissionAsync(AssignRolePermissionDto input);
    Task AssignUserSystemScopeAsync(AssignUserSystemScopeDto input);
}