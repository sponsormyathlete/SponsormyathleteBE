using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Deals;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[Route("api/deals")]
public class DealsController(IUserSyncService userSyncService, IDealService dealService)
    : ApiControllerBase(userSyncService)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DealDto>>> GetMine(CancellationToken ct) =>
        Ok(await dealService.GetDealsForUserAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpGet("{dealId:guid}")]
    public async Task<ActionResult<DealDto>> Get(Guid dealId, CancellationToken ct) =>
        Ok(await dealService.GetDealAsync(await GetCurrentUserIdAsync(ct), dealId, ct));

    [HttpPost("with-athlete/{athleteProfileId:guid}")]
    public async Task<ActionResult<DealDto>> Propose(Guid athleteProfileId, [FromBody] ProposeDealRequest request, CancellationToken ct) =>
        Ok(await dealService.ProposeDealAsync(await GetCurrentUserIdAsync(ct), athleteProfileId, request, ct));

    [HttpPost("{dealId:guid}/deliverables-submitted")]
    public async Task<ActionResult<DealDto>> MarkDeliverablesSubmitted(Guid dealId, CancellationToken ct) =>
        Ok(await dealService.MarkDeliverablesSubmittedAsync(await GetCurrentUserIdAsync(ct), dealId, ct));
}
