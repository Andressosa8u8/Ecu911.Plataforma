namespace Ecu911.AuthService.DTOs;

public class LoginDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string SystemCode { get; set; } = default!;
}