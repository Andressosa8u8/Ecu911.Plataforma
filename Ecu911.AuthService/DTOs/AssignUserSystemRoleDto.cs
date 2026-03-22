namespace Ecu911.AuthService.DTOs;

public class AssignUserSystemRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid SystemModuleId { get; set; }
}