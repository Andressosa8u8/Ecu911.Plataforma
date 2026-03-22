namespace Ecu911.AuthService.Models;

public class SystemModule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserSystemRole> UserSystemRoles { get; set; } = new List<UserSystemRole>();
    public ICollection<UserSystemScope> UserSystemScopes { get; set; } = new List<UserSystemScope>();
}