using System.Security.Claims;

namespace LoanTracker.Helpers;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? throw new InvalidOperationException("No user id claim."));

    public static string GetDisplayName(this ClaimsPrincipal user) =>
        user.FindFirstValue("DisplayName") ?? user.Identity?.Name ?? "User";
}
