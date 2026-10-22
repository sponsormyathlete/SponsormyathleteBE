using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Application.Deals;

public record DealDto(
    Guid Id,
    string CounterpartyLabel,
    string Title,
    string? Note,
    DealType Type,
    decimal DeclaredValue,
    decimal PlatformFeeAmount,
    decimal TotalAmount,
    DealStatus Status,
    string? StripePaymentLinkUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    DateTimeOffset? CompletedAt
);

/// <summary>Everything an admin needs to run the concierge flow — real identities, both sides.</summary>
public record AdminDealDto(
    Guid Id,
    Guid AthleteUserId,
    string AthleteEmail,
    string? AthleteDisplayName,
    Guid SponsorUserId,
    string SponsorEmail,
    string? SponsorDisplayName,
    string Title,
    string? Note,
    DealType Type,
    decimal DeclaredValue,
    decimal PlatformFeeAmount,
    decimal TotalAmount,
    DealStatus Status,
    string? StripePaymentLinkUrl,
    string? AdminNotes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    DateTimeOffset? CompletedAt
);

/// <param name="DeclaredValue">Cash amount offered, or the declared retail value for a product/service deal (brief 2.2/2.3).</param>
public record ProposeDealRequest(Guid? SponsorshipPackageId, string Title, DealType Type, decimal DeclaredValue, string? Note);

public record AdminNoteRequest(string? AdminNotes);
