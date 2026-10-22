namespace SponsorMyAthlete.Application.Deals;

/// <summary>
/// Phase 1 concierge deal flow (brief 3.3) — no escrow, no self-service checkout. A sponsor
/// proposes, an admin manually agrees it and generates a Stripe Payment Link, the sponsor pays
/// through that link (webhook marks it Paid), and the admin releases payment to the athlete by
/// bank transfer once deliverables are confirmed — all outside this API except status tracking.
/// </summary>
public interface IDealService
{
    Task<DealDto> ProposeDealAsync(Guid sponsorUserId, Guid athleteProfileId, ProposeDealRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<DealDto>> GetDealsForUserAsync(Guid userId, CancellationToken ct = default);
    Task<DealDto> GetDealAsync(Guid userId, Guid dealId, CancellationToken ct = default);

    /// <summary>Athlete confirms they've delivered their side — admin still confirms before releasing payment.</summary>
    Task<DealDto> MarkDeliverablesSubmittedAsync(Guid userId, Guid dealId, CancellationToken ct = default);

    Task<IReadOnlyList<AdminDealDto>> GetAllDealsAsync(CancellationToken ct = default);
    Task<AdminDealDto> AgreeAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default);
    Task<AdminDealDto> GeneratePaymentLinkAsync(Guid dealId, CancellationToken ct = default);
    Task<AdminDealDto> CompleteAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default);
    Task<AdminDealDto> CancelAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default);

    /// <summary>Called by the Stripe webhook when a checkout session for a deal's payment link completes.</summary>
    Task MarkPaidByPaymentLinkIdAsync(string stripePaymentLinkId, CancellationToken ct = default);
}
