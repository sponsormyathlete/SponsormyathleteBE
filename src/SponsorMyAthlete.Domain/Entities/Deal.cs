namespace SponsorMyAthlete.Domain.Entities;

/// <summary>A concierge-assisted deal (brief 3.3) — the operator advances status manually; no escrow automation in Phase 1.</summary>
public class Deal
{
    public Guid Id { get; set; }
    public Guid AthleteUserId { get; set; }
    public Guid SponsorUserId { get; set; }
    public Guid? MessageThreadId { get; set; }

    public DealType Type { get; set; }
    public decimal DeclaredValue { get; set; }
    public decimal PlatformFeeAmount { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Proposed;

    public string? StripePaymentLinkId { get; set; }
    public string? StripePaymentLinkUrl { get; set; }
    public string? AdminNotes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
