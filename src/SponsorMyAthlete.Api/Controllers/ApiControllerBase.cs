using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase(IUserSyncService userSyncService) : ControllerBase
{
    protected string CurrentAuth0Sub =>
        User.FindFirst("sub")?.Value ?? throw new InvalidOperationException("Token is missing a 'sub' claim.");

    /// <summary>Resolves the caller's internal user id. Returns null if they haven't hit /api/users/me/sync yet.</summary>
    protected async Task<Guid?> TryGetCurrentUserIdAsync(CancellationToken ct = default)
    {
        var user = await userSyncService.GetByAuth0SubAsync(CurrentAuth0Sub, ct);
        return user?.Id;
    }

    protected async Task<Guid> GetCurrentUserIdAsync(CancellationToken ct = default)
    {
        var userId = await TryGetCurrentUserIdAsync(ct);
        if (userId is null)
            throw new InvalidOperationException("User has not been synced. Call POST /api/users/me/sync first.");
        return userId.Value;
    }
}
