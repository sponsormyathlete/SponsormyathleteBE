using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.SponsorProfiles;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[Route("api/sponsor-profile")]
public class SponsorProfilesController(IUserSyncService userSyncService, ISponsorProfileService profileService)
    : ApiControllerBase(userSyncService)
{
    [HttpGet("me")]
    public async Task<ActionResult<SponsorProfileDto>> GetMine(CancellationToken ct) =>
        Ok(await profileService.GetOrCreateForUserAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpPut("me/interests")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateInterests([FromBody] UpdateSponsorInterestsRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateInterestsAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/budget")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateBudget([FromBody] UpdateSponsorBudgetRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateBudgetAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/athlete-count")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateAthleteCount([FromBody] UpdateSponsorAthleteCountRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateAthleteCountAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/deliverables")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateDeliverables([FromBody] UpdateSponsorDeliverablesRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateDeliverablesAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/location")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateLocation([FromBody] UpdateSponsorLocationRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateLocationAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/event-preference")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateEventPreference([FromBody] UpdateSponsorEventPrefRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateEventPreferenceAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/athlete-level-preference")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateAthleteLevelPreference([FromBody] UpdateSponsorAthleteLevelRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateAthleteLevelPreferenceAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/reach-importance")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateReachImportance([FromBody] UpdateSponsorReachImportanceRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateReachImportanceAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/sponsorship-type")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateSponsorshipType([FromBody] UpdateSponsorTypeRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateSponsorshipTypeAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/relationship-style")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateRelationshipStyle([FromBody] UpdateSponsorRelationshipStyleRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateRelationshipStyleAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/about")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateAbout([FromBody] UpdateSponsorAboutRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateAboutAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/identity")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateIdentity([FromBody] UpdateSponsorIdentityRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateIdentityAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/wishlists")]
    public async Task<ActionResult<SponsorProfileDto>> UpdateWishlists([FromBody] UpdateSponsorWishlistsRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateWishlistsAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpGet("me/public")]
    public async Task<ActionResult<PublicSponsorProfileDto>> GetMinePublic(CancellationToken ct) =>
        Ok(await profileService.GetPublicForUserAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpGet("by-thread/{threadId:guid}")]
    public async Task<ActionResult<PublicSponsorProfileDto>> GetForThread(Guid threadId, CancellationToken ct) =>
        Ok(await profileService.GetPublicForThreadAsync(await GetCurrentUserIdAsync(ct), threadId, ct));

    [HttpGet("me/saved-athletes")]
    public async Task<ActionResult<IReadOnlyList<PublicAthleteProfileDto>>> GetSavedAthletes(CancellationToken ct) =>
        Ok(await profileService.GetSavedAthletesAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpPut("me/saved-athletes/{athleteProfileId:guid}")]
    public async Task<IActionResult> SaveAthlete(Guid athleteProfileId, CancellationToken ct)
    {
        await profileService.SaveAthleteAsync(await GetCurrentUserIdAsync(ct), athleteProfileId, ct);
        return NoContent();
    }

    [HttpDelete("me/saved-athletes/{athleteProfileId:guid}")]
    public async Task<IActionResult> UnsaveAthlete(Guid athleteProfileId, CancellationToken ct)
    {
        await profileService.UnsaveAthleteAsync(await GetCurrentUserIdAsync(ct), athleteProfileId, ct);
        return NoContent();
    }
}
