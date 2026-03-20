namespace Ecu911.AuthService.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
    public List<string> Roles { get; set; } = new();
}