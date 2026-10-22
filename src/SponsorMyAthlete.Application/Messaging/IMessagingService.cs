namespace SponsorMyAthlete.Application.Messaging;

public interface IMessagingService
{
    Task<IReadOnlyList<ThreadSummaryDto>> GetThreadsForUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Sponsor-initiated (brief 3.3 step 1) — finds or creates the thread with the athlete who owns this profile.</summary>
    Task<ThreadDetailDto> GetOrCreateThreadWithAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default);

    /// <summary>
    /// Athlete-initiated — finds or creates the thread with the sponsor who owns this profile. The
    /// sponsor's identity still only reveals to the athlete once the sponsor replies (brief 5.4).
    /// </summary>
    Task<ThreadDetailDto> GetOrCreateThreadWithSponsorAsync(Guid athleteUserId, Guid sponsorProfileId, CancellationToken ct = default);

    Task<ThreadDetailDto> GetThreadAsync(Guid userId, Guid threadId, CancellationToken ct = default);

    /// <summary>Runs the message through the anti-disintermediation filter before persisting (brief 7a.2).</summary>
    Task<SendMessageResult> SendMessageAsync(Guid userId, Guid threadId, SendMessageRequest request, CancellationToken ct = default);
}
