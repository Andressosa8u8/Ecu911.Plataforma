namespace Ecu911.AuthService.Models;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}