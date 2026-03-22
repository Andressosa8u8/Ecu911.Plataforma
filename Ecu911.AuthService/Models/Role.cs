namespace Ecu911.AuthService.Models;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserSystemRole> UserSystemRoles { get; set; } = new List<UserSystemRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}