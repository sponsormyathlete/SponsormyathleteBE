using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Application.AthleteProfiles;

public record AccomplishmentDto(Guid Id, string Description, int? Year);
public record AthletePhotoDto(Guid Id, string Url, bool IsProfilePhoto);
public record SponsorshipPackageDto(Guid Id, string Title, string Description);

public record AthleteProfileDto(
    Guid Id,
    Guid UserId,
    string? DisplayName,
    string? Sport,
    CompetitiveLevel? CompetitiveLevel,
    string? Organisation,
    string? State,
    DateOnly? DateOfBirth,
    string? Gender,
    string? WeightClass,
    string? Bio,
    IReadOnlyList<AccomplishmentDto> Accomplishments,
    IReadOnlyList<AthletePhotoDto> Photos,
    string? InstagramHandle,
    string? FacebookHandle,
    int? FollowerCount,
    int? AverageReach,
    IReadOnlyList<SponsorshipPackageDto> SponsorshipPackages,
    string? VerificationReference,
    VerificationStatus VerificationStatus,
    string? VerificationAdminNotes,
    SubscriptionStatus SubscriptionStatus,
    int ProfileStrengthScore
);

/// <summary>
/// What sponsors and the public see in the directory. Leaves out verification and billing data,
/// date of birth (age only), and social handles — handles are a contact channel, so only the
/// audience numbers are shown (brief 7a.1).
/// </summary>
public record PublicAthleteProfileDto(
    Guid Id,
    string DisplayName,
    string? Sport,
    CompetitiveLevel? CompetitiveLevel,
    string? Organisation,
    string? State,
    int? Age,
    string? Gender,
    string? WeightClass,
    string? Bio,
    IReadOnlyList<AccomplishmentDto> Accomplishments,
    IReadOnlyList<AthletePhotoDto> Photos,
    bool HasInstagram,
    bool HasFacebook,
    int? FollowerCount,
    int? AverageReach,
    IReadOnlyList<SponsorshipPackageDto> SponsorshipPackages
);

public record UpdateBasicsRequest(string Sport, CompetitiveLevel CompetitiveLevel, string Organisation, string State);
public record UpdatePersonalDetailsRequest(string DisplayName, DateOnly DateOfBirth, string Gender, string WeightClass);
public record UpdateBioRequest(string Bio);
public record AccomplishmentInput(string Description, int? Year);
public record UpdateAccomplishmentsRequest(List<AccomplishmentInput> Accomplishments);
public record AddPhotoRequest(string Url, bool IsProfilePhoto);
public record UpdateSocialMediaRequest(string? InstagramHandle, string? FacebookHandle, int? FollowerCount, int? AverageReach);
public record PackageInput(string Title, string Description);
public record UpdateSponsorshipPackagesRequest(List<PackageInput> Packages);
public record SubmitVerificationRequest(string Reference);
public record ReviewVerificationRequest(string? AdminNotes);
