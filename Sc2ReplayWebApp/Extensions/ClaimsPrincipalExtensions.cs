using System.Security.Claims;

namespace Sc2ReplayWebApp.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? BattleTag(
        this ClaimsPrincipal user)
    {
        return user.FindFirst(
            ClaimTypes.Name)?.Value;
    }

    public static string? BattleNetId(
        this ClaimsPrincipal user)
    {
        return user.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;
    }
}
