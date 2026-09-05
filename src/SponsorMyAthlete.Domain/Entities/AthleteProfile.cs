namespace SponsorMyAthlete.Domain.Entities;

public class AthleteProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Step 1 — sport
    public string? Sport { get; set; }

    // Step 2 — competitive level
    public CompetitiveLevel? CompetitiveLevel { get; set; }

    // Step 3 — organisation / sanctioning body / club
    public string? Organisation { get; set; }

    // Step 4 — state / location
    public string? State { get; set; }

    // Step 5 — personal details
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? WeightClass { get; set; }

    // Step 6 — bio
    public string? Bio { get; set; }

    // Step 7 — accomplishments
    public List<AthleteAccomplishment> Accomplishments { get; set; } = new();

    // Step 8 — photos
    public List<AthletePhoto> Photos { get; set; } = new();

    // Step 9 — social media
    public string? InstagramHandle { get; set; }
    public string? FacebookHandle { get; set; }
    public int? FollowerCount { get; set; }
    public int? AverageReach { get; set; }

    // Step 10 — sponsorship packages offered
    public List<SponsorshipPackage> SponsorshipPackages { get; set; } = new();

    // Step 11 — verification
    public string? VerificationReference { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Draft;
    public string? VerificationAdminNotes { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }

    // Step 12 — subscription
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.None;
    public string? StripeSubscriptionId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
