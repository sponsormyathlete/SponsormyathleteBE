using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.AthleteProfiles;

namespace SponsorMyAthlete.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/directory/athletes")]
public class AthleteDirectoryController(IAthleteProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AthleteProfileDto>>> Search(
        [FromQuery] string? sport, [FromQuery] string? state, CancellationToken ct) =>
        Ok(await profileService.SearchDirectoryAsync(sport, state, ct));

    [HttpGet("{athleteProfileId:guid}")]
    public async Task<ActionResult<AthleteProfileDto>> Get(Guid athleteProfileId, CancellationToken ct)
    {
        var profile = await profileService.GetPublishedByIdAsync(athleteProfileId, ct);
        return profile is null ? NotFound() : Ok(profile);
    }
}
