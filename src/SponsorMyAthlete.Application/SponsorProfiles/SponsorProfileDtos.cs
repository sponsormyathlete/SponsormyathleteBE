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
    bool IsBusiness,
    string? Abn,
    string? AbnEntityName,
    DateTimeOffset? AbnValidatedAt,
    IReadOnlyList<SponsorWishlistDto> Wishlists
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
public record UpdateSponsorIdentityRequest(bool IsBusiness, string? Abn);
public record WishlistInput(string Title, string Description);
public record UpdateSponsorWishlistsRequest(List<WishlistInput> Wishlists);
