namespace Ecu911.AuthService.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Guid? OrganizationalUnitId { get; set; }
}