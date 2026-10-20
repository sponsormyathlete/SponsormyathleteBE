using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Api.Auth;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.ProfileViews;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/directory/athletes")]
public class AthleteDirectoryController(
    IAthleteProfileService profileService,
    IProfileViewService profileViewService,
    IUserSyncService userSyncService,
    ViewerKeyHasher viewerKeys) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublicAthleteProfileDto>>> Search(
        [FromQuery] string? sport, [FromQuery] string? state, [FromQuery] CompetitiveLevel? competitiveLevel, CancellationToken ct) =>
        Ok(await profileService.SearchDirectoryAsync(sport, state, competitiveLevel, ct));

    [HttpGet("{athleteProfileId:guid}")]
    public async Task<ActionResult<PublicAthleteProfileDto>> Get(Guid athleteProfileId, CancellationToken ct)
    {
        var profile = await profileService.GetPublishedByIdAsync(athleteProfileId, ct);
        return profile is null ? NotFound() : Ok(profile);
    }

    /// <summary>
    /// Brief 5.1 view tracking. Separate from GET so refetches and prefetches don't count — the
    /// profile page posts once per visit. Anonymous is fine; a valid token attributes the view.
    /// </summary>
    [HttpPost("{athleteProfileId:guid}/views")]
    public async Task<IActionResult> RecordView(Guid athleteProfileId, CancellationToken ct)
    {
        var sub = User.FindFirst("sub")?.Value;
        var viewer = sub is null ? null : await userSyncService.GetByAuth0SubAsync(sub, ct);
        var viewerKey = viewer is null ? viewerKeys.ForAnonymous(HttpContext) : viewerKeys.ForUser(viewer.Id);

        await profileViewService.RecordAsync(athleteProfileId, viewer?.Id, viewerKey, ct);
        return NoContent();
    }
}
