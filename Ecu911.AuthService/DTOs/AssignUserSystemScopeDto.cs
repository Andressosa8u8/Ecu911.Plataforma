namespace Ecu911.AuthService.DTOs;

public class AssignUserSystemScopeDto
{
    public Guid UserId { get; set; }
    public Guid SystemModuleId { get; set; }
    public string ScopeLevel { get; set; } = default!;
    public string? CenterCode { get; set; }
    public string? JurisdictionCode { get; set; }
}