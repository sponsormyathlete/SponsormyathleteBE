using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[Route("api/athlete-profile")]
public class AthleteProfilesController(IUserSyncService userSyncService, IAthleteProfileService profileService)
    : ApiControllerBase(userSyncService)
{
    [HttpGet("me")]
    public async Task<ActionResult<AthleteProfileDto>> GetMine(CancellationToken ct) =>
        Ok(await profileService.GetOrCreateForUserAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpPut("me/basics")]
    public async Task<ActionResult<AthleteProfileDto>> UpdateBasics([FromBody] UpdateBasicsRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateBasicsAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/personal-details")]
    public async Task<ActionResult<AthleteProfileDto>> UpdatePersonalDetails([FromBody] UpdatePersonalDetailsRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdatePersonalDetailsAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/bio")]
    public async Task<ActionResult<AthleteProfileDto>> UpdateBio([FromBody] UpdateBioRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateBioAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/accomplishments")]
    public async Task<ActionResult<AthleteProfileDto>> UpdateAccomplishments([FromBody] UpdateAccomplishmentsRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateAccomplishmentsAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPost("me/photos")]
    public async Task<ActionResult<AthleteProfileDto>> AddPhoto([FromBody] AddPhotoRequest request, CancellationToken ct) =>
        Ok(await profileService.AddPhotoAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpDelete("me/photos/{photoId:guid}")]
    public async Task<ActionResult<AthleteProfileDto>> RemovePhoto(Guid photoId, CancellationToken ct) =>
        Ok(await profileService.RemovePhotoAsync(await GetCurrentUserIdAsync(ct), photoId, ct));

    [HttpPut("me/social-media")]
    public async Task<ActionResult<AthleteProfileDto>> UpdateSocialMedia([FromBody] UpdateSocialMediaRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateSocialMediaAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPut("me/sponsorship-packages")]
    public async Task<ActionResult<AthleteProfileDto>> UpdateSponsorshipPackages([FromBody] UpdateSponsorshipPackagesRequest request, CancellationToken ct) =>
        Ok(await profileService.UpdateSponsorshipPackagesAsync(await GetCurrentUserIdAsync(ct), request, ct));

    [HttpPost("me/submit-verification")]
    public async Task<ActionResult<AthleteProfileDto>> SubmitVerification([FromBody] SubmitVerificationRequest request, CancellationToken ct) =>
        Ok(await profileService.SubmitVerificationAsync(await GetCurrentUserIdAsync(ct), request, ct));
}
