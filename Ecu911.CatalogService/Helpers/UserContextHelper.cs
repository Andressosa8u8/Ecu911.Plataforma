using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ecu911.CatalogService.Helpers;

public static class UserContextHelper
{
    public static string? GetUsername(ClaimsPrincipal user)
    {
        return user.Identity?.Name
            ?? user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
            ?? user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public static Guid? GetOrganizationalUnitId(ClaimsPrincipal user)
    {
        var claimValue = user.Claims
            .FirstOrDefault(c => c.Type == "organizationalUnitId")
            ?.Value;

        if (Guid.TryParse(claimValue, out var organizationalUnitId))
        {
            return organizationalUnitId;
        }

        return null;
    }

    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole("ADMIN");
    }
}