using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Application.SponsorProfiles;

public record SponsorWishlistDto(Guid Id, string Title, string Description);

public record SponsorProfileDto(
    Guid Id,
    Guid UserId,
    List<string> SportsOfInterest,
    decimal BudgetMin,
    decimal BudgetMax,
    int AthleteCountMin,
    int? AthleteCountMax,
    List<string> KeyDeliverablesWanted,
    string? State,
    EventAttendancePreference AttendEvents,
    AthleteLevelPreference AthleteLevelPreference,
    ReachImportance ReachImportance,
    SponsorshipType SponsorshipType,
    RelationshipStyle RelationshipStyle,
    string? AboutBlurb,
    string? DisplayName,
    bool IsBusiness,
    string? Abn,
    string? AbnEntityName,
    DateTimeOffset? AbnValidatedAt,
    IReadOnlyList<SponsorWishlistDto> Wishlists
);

/// <summary>
/// What an athlete sees about a sponsor who has messaged them. No email, ABN number, or other
/// contact details (brief 7a.1) — only whether the business was verified against the ABR.
/// </summary>
public record PublicSponsorProfileDto(
    Guid Id,
    string DisplayName,
    bool IsBusiness,
    bool AbnVerified,
    List<string> SportsOfInterest,
    decimal BudgetMin,
    decimal BudgetMax,
    int AthleteCountMin,
    int? AthleteCountMax,
    List<string> KeyDeliverablesWanted,
    string? State,
    EventAttendancePreference AttendEvents,
    AthleteLevelPreference AthleteLevelPreference,
    ReachImportance ReachImportance,
    SponsorshipType SponsorshipType,
    RelationshipStyle RelationshipStyle,
    string? AboutBlurb,
    IReadOnlyList<SponsorWishlistDto> Wishlists
);

/// <summary>
/// Directory card for an athlete browsing sponsors. No display name or business identity —
/// sponsors stay anonymous until they message an athlete first (brief 5.4); this mirrors that
/// for the reverse direction so an athlete-initiated message still triggers the normal reveal-on-reply flow.
/// </summary>
public record PublicSponsorSummaryDto(
    Guid Id,
    bool IsBusiness,
    List<string> SportsOfInterest,
    decimal BudgetMin,
    decimal BudgetMax,
    string? State,
    AthleteLevelPreference AthleteLevelPreference,
    SponsorshipType SponsorshipType,
    RelationshipStyle RelationshipStyle,
    List<string> KeyDeliverablesWanted,
    string? AboutBlurb
);

public record UpdateSponsorInterestsRequest(List<string> SportsOfInterest);
public record UpdateSponsorBudgetRequest(decimal BudgetMin, decimal BudgetMax);
public record UpdateSponsorAthleteCountRequest(int AthleteCountMin, int? AthleteCountMax);
public record UpdateSponsorDeliverablesRequest(List<string> KeyDeliverablesWanted);
public record UpdateSponsorLocationRequest(string State);
public record UpdateSponsorEventPrefRequest(EventAttendancePreference AttendEvents);
public record UpdateSponsorAthleteLevelRequest(AthleteLevelPreference AthleteLevelPreference);
public record UpdateSponsorReachImportanceRequest(ReachImportance ReachImportance);
public record UpdateSponsorTypeRequest(SponsorshipType SponsorshipType);
public record UpdateSponsorRelationshipStyleRequest(RelationshipStyle RelationshipStyle);
public record UpdateSponsorAboutRequest(string AboutBlurb);
public record UpdateSponsorIdentityRequest(bool IsBusiness, string? Abn, string? DisplayName);
public record WishlistInput(string Title, string Description);
public record UpdateSponsorWishlistsRequest(List<WishlistInput> Wishlists);
