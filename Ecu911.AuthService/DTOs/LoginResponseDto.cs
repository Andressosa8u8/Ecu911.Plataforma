namespace Ecu911.AuthService.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public DateTime Expiration { get; set; }
    public UserDto User { get; set; } = default!;
}