using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Messaging;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[Route("api/messaging")]
public class MessagingController(IUserSyncService userSyncService, IMessagingService messagingService)
    : ApiControllerBase(userSyncService)
{
    [HttpGet("threads")]
    public async Task<ActionResult<IReadOnlyList<ThreadSummaryDto>>> GetThreads(CancellationToken ct) =>
        Ok(await messagingService.GetThreadsForUserAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpGet("threads/{threadId:guid}")]
    public async Task<ActionResult<ThreadDetailDto>> GetThread(Guid threadId, CancellationToken ct) =>
        Ok(await messagingService.GetThreadAsync(await GetCurrentUserIdAsync(ct), threadId, ct));

    [HttpPost("threads/with-athlete/{athleteProfileId:guid}")]
    public async Task<ActionResult<ThreadDetailDto>> StartThreadWithAthlete(Guid athleteProfileId, CancellationToken ct) =>
        Ok(await messagingService.GetOrCreateThreadWithAthleteAsync(await GetCurrentUserIdAsync(ct), athleteProfileId, ct));

    [HttpPost("threads/{threadId:guid}/messages")]
    public async Task<ActionResult<SendMessageResult>> SendMessage(Guid threadId, [FromBody] SendMessageRequest request, CancellationToken ct) =>
        Ok(await messagingService.SendMessageAsync(await GetCurrentUserIdAsync(ct), threadId, request, ct));
}
