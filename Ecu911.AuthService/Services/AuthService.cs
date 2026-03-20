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

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        AuthDbContext context,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
        _configuration = configuration;
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
            Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList()
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
}