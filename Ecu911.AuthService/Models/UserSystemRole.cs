namespace Ecu911.AuthService.Models;

public class UserSystemRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public Guid SystemModuleId { get; set; }
    public SystemModule SystemModule { get; set; } = default!;
}