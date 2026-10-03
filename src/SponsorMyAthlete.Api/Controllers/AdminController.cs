using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Admin;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Api.Controllers;

/// <summary>
/// Admin role lives in our Users table, not as an Auth0/JWT role claim, so authorization here
/// is a manual DB check rather than an [Authorize(Roles=...)] policy.
/// </summary>
[Route("api/admin")]
public class AdminController(IUserSyncService userSyncService, IAthleteProfileService profileService, IAdminFlagService flagService)
    : ApiControllerBase(userSyncService)
{
    [HttpGet("verifications/pending")]
    public async Task<ActionResult<IReadOnlyList<AthleteProfileDto>>> PendingVerifications(CancellationToken ct)
    {
        var forbidden = await RequireAdminAsync(ct);
        if (forbidden is not null) return forbidden;
        return Ok(await profileService.GetPendingVerificationsAsync(ct));
    }

    [HttpPost("verifications/{athleteProfileId:guid}/approve")]
    public async Task<ActionResult<AthleteProfileDto>> Approve(Guid athleteProfileId, [FromBody] ReviewVerificationRequest request, CancellationToken ct)
    {
        var forbidden = await RequireAdminAsync(ct);
        if (forbidden is not null) return forbidden;
        return Ok(await profileService.ApproveVerificationAsync(athleteProfileId, request, ct));
    }

    [HttpPost("verifications/{athleteProfileId:guid}/reject")]
    public async Task<ActionResult<AthleteProfileDto>> Reject(Guid athleteProfileId, [FromBody] ReviewVerificationRequest request, CancellationToken ct)
    {
        var forbidden = await RequireAdminAsync(ct);
        if (forbidden is not null) return forbidden;
        return Ok(await profileService.RejectVerificationAsync(athleteProfileId, request, ct));
    }

    [HttpGet("flags")]
    public async Task<ActionResult<IReadOnlyList<AdminFlagDto>>> GetFlags([FromQuery] bool includeResolved, CancellationToken ct)
    {
        var forbidden = await RequireAdminAsync(ct);
        if (forbidden is not null) return forbidden;
        return Ok(await flagService.GetFlagsAsync(includeResolved, ct));
    }

    [HttpPost("flags/{flagId:guid}/resolve")]
    public async Task<IActionResult> ResolveFlag(Guid flagId, CancellationToken ct)
    {
        var forbidden = await RequireAdminAsync(ct);
        if (forbidden is not null) return forbidden;
        await flagService.ResolveFlagAsync(flagId, ct);
        return NoContent();
    }

    private async Task<ActionResult?> RequireAdminAsync(CancellationToken ct)
    {
        var user = await userSyncService.GetByAuth0SubAsync(CurrentAuth0Sub, ct);
        return user?.Role == UserRole.Admin ? null : Forbid();
    }
}
