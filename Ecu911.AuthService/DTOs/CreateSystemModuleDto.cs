namespace Ecu911.AuthService.DTOs;

public class CreateSystemModuleDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}