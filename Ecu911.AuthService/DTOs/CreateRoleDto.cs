namespace Ecu911.AuthService.DTOs;

public class CreateRoleDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
}