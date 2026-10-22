using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.SponsorProfiles;

namespace SponsorMyAthlete.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/directory/sponsors")]
public class SponsorDirectoryController(ISponsorProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublicSponsorSummaryDto>>> Search(
        [FromQuery] string? sport, [FromQuery] string? state, CancellationToken ct) =>
        Ok(await profileService.SearchDirectoryAsync(sport, state, ct));

    [HttpGet("{sponsorProfileId:guid}")]
    public async Task<ActionResult<PublicSponsorSummaryDto>> Get(Guid sponsorProfileId, CancellationToken ct)
    {
        var profile = await profileService.GetSummaryByIdAsync(sponsorProfileId, ct);
        return profile is null ? NotFound() : Ok(profile);
    }
}
