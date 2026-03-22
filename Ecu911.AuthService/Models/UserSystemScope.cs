namespace Ecu911.AuthService.Models;

public class UserSystemScope
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid SystemModuleId { get; set; }
    public SystemModule SystemModule { get; set; } = default!;

    public string ScopeLevel { get; set; } = "LOCAL";
    public string? CenterCode { get; set; }
    public string? JurisdictionCode { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}