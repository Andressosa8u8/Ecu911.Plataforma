using Ecu911.AuthService.Data;
using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Helpers;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecu911.AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ISystemModuleRepository _systemModuleRepository;
    private readonly IUserSystemRoleRepository _userSystemRoleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserSystemScopeRepository _userSystemScopeRepository;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        AuthDbContext context,
        IConfiguration configuration,
        ISystemModuleRepository systemModuleRepository,
        IUserSystemRoleRepository userSystemRoleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserSystemScopeRepository userSystemScopeRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
        _configuration = configuration;
        _systemModuleRepository = systemModuleRepository;
        _userSystemRoleRepository = userSystemRoleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userSystemScopeRepository = userSystemScopeRepository;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto input)
    {
        var existing = await _userRepository.GetByUsernameAsync(input.Username);
        if (existing != null)
            throw new Exception("El nombre de usuario ya existe.");

        var user = new User
        {
            Username = input.Username,
            FullName = input.FullName,
            Email = input.Email,
            PasswordHash = PasswordHelper.HashPassword(input.Password),
            IsActive = true,
            OrganizationalUnitId = input.OrganizationalUnitId
        };

        var created = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = created.Id,
            Username = created.Username,
            FullName = created.FullName,
            Email = created.Email,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            OrganizationalUnitId = created.OrganizationalUnitId,
            Roles = new List<string>()
        };
    }

    public async Task<Role> CreateRoleAsync(CreateRoleDto input)
    {
        var existing = await _roleRepository.GetByNameAsync(input.Name);
        if (existing != null)
            throw new Exception("El rol ya existe.");

        var role = new Role
        {
            Name = input.Name,
            Description = input.Description
        };

        return await _roleRepository.AddAsync(role);
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var exists = await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol.");

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(x => new UserDto
        {
            Id = x.Id,
            Username = x.Username,
            FullName = x.FullName,
            Email = x.Email,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            OrganizationalUnitId = x.OrganizationalUnitId,
            Roles = x.UserRoles.Select(ur => ur.Role.Name).Distinct().ToList(),
            Systems = x.UserSystemRoles
                .Where(usr => usr.SystemModule != null)
                .Select(usr => usr.SystemModule.Code)
                .Distinct()
                .ToList(),
            CurrentSystem = null
        }).ToList();
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto input)
    {
        var user = await _userRepository.GetByUsernameAsync(input.Username);

        if (user == null || !user.IsActive)
            return null;

        if (!PasswordHelper.VerifyPassword(input.Password, user.PasswordHash))
            return null;

        var roles = user.UserRoles.Select(x => x.Role.Name).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("fullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        if (user.OrganizationalUnitId.HasValue)
        {
            claims.Add(new Claim("organizationalUnitId", user.OrganizationalUnitId.Value.ToString()));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);
        var expiration = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                OrganizationalUnitId = user.OrganizationalUnitId,
                Roles = roles
            }
        };
    }

    public async Task<SystemModuleDto> CreateSystemModuleAsync(CreateSystemModuleDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del sistema es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del sistema es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existing = await _systemModuleRepository.GetByCodeAsync(normalizedCode);
        if (existing != null)
            throw new Exception("Ya existe un sistema con ese código.");

        var entity = new SystemModule
        {
            Code = normalizedCode,
            Name = input.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description)
                ? null
                : input.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _systemModuleRepository.AddAsync(entity);

        return new SystemModuleDto
        {
            Id = created.Id,
            Code = created.Code,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<SystemModuleDto>> GetSystemModulesAsync()
    {
        var systems = await _systemModuleRepository.GetAllAsync();

        return systems.Select(x => new SystemModuleDto
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task AssignUserSystemRoleAsync(AssignUserSystemRoleDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null || !system.IsActive)
            throw new Exception("Sistema no encontrado o inactivo.");

        var exists = await _userSystemRoleRepository.ExistsAsync(input.UserId, input.RoleId, input.SystemModuleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol asignado para ese sistema.");

        await _userSystemRoleRepository.AddAsync(new UserSystemRole
        {
            UserId = input.UserId,
            RoleId = input.RoleId,
            SystemModuleId = input.SystemModuleId
        });
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del permiso es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del permiso es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existing = await _permissionRepository.GetByCodeAsync(normalizedCode);
        if (existing != null)
            throw new Exception("Ya existe un permiso con ese código.");

        var entity = new Permission
        {
            Code = normalizedCode,
            Name = input.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _permissionRepository.AddAsync(entity);

        return new PermissionDto
        {
            Id = created.Id,
            Code = created.Code,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<PermissionDto>> GetPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        return permissions.Select(x => new PermissionDto
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task AssignRolePermissionAsync(AssignRolePermissionDto input)
    {
        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var permission = await _permissionRepository.GetByIdAsync(input.PermissionId);
        if (permission == null || !permission.IsActive)
            throw new Exception("Permiso no encontrado o inactivo.");

        var exists = await _rolePermissionRepository.ExistsAsync(input.RoleId, input.PermissionId);
        if (exists)
            throw new Exception("El rol ya tiene asignado ese permiso.");

        await _rolePermissionRepository.AddAsync(new RolePermission
        {
            RoleId = input.RoleId,
            PermissionId = input.PermissionId
        });
    }

    public async Task AssignUserSystemScopeAsync(AssignUserSystemScopeDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null || !system.IsActive)
            throw new Exception("Sistema no encontrado o inactivo.");

        if (string.IsNullOrWhiteSpace(input.ScopeLevel))
            throw new Exception("El nivel de alcance es obligatorio.");

        var normalizedScope = input.ScopeLevel.Trim().ToUpperInvariant();

        var allowed = new[] { "LOCAL", "JURISDICCION", "NACIONAL", "EXTERNO" };
        if (!allowed.Contains(normalizedScope))
            throw new Exception("El nivel de alcance no es válido.");

        var existing = await _userSystemScopeRepository.GetByUserAndSystemAsync(input.UserId, input.SystemModuleId);

        if (existing == null)
        {
            await _userSystemScopeRepository.AddAsync(new UserSystemScope
            {
                UserId = input.UserId,
                SystemModuleId = input.SystemModuleId,
                ScopeLevel = normalizedScope,
                CenterCode = string.IsNullOrWhiteSpace(input.CenterCode) ? null : input.CenterCode.Trim().ToUpperInvariant(),
                JurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode) ? null : input.JurisdictionCode.Trim().ToUpperInvariant(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            return;
        }

        existing.ScopeLevel = normalizedScope;
        existing.CenterCode = string.IsNullOrWhiteSpace(input.CenterCode) ? null : input.CenterCode.Trim().ToUpperInvariant();
        existing.JurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode) ? null : input.JurisdictionCode.Trim().ToUpperInvariant();
        existing.IsActive = true;

        await _userSystemScopeRepository.UpdateAsync(existing);
    }
}