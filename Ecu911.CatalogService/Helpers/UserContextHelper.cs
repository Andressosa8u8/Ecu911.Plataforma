using System.Security.Claims;

namespace Ecu911.CatalogService.Helpers;

public static class UserContextHelper
{
    public static string? GetUsername(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value
            ?? user.FindFirst("unique_name")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}