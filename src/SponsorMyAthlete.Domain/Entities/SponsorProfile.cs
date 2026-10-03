namespace SponsorMyAthlete.Domain.Entities;

public class SponsorProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Step 1 — sports of interest
    public List<string> SportsOfInterest { get; set; } = new();

    // Step 2 — sponsorship budget range
    public decimal BudgetMin { get; set; }
    public decimal BudgetMax { get; set; }

    // Step 3 — number of athletes
    public int AthleteCountMin { get; set; } = 1;
    public int? AthleteCountMax { get; set; }

    // Step 4 — key deliverables wanted
    public List<string> KeyDeliverablesWanted { get; set; } = new();

    // Step 5 — state / location
    public string? State { get; set; }

    // Step 6 — attend athlete events
    public EventAttendancePreference AttendEvents { get; set; } = EventAttendancePreference.Maybe;

    // Step 7 — athlete level preference
    public AthleteLevelPreference AthleteLevelPreference { get; set; } = AthleteLevelPreference.All;

    // Step 8 — social media reach importance
    public ReachImportance ReachImportance { get; set; } = ReachImportance.NotImportant;

    // Step 9 — sponsorship type
    public SponsorshipType SponsorshipType { get; set; } = SponsorshipType.Cash;

    // Step 10 — relationship style
    public RelationshipStyle RelationshipStyle { get; set; } = RelationshipStyle.Transactional;

    // Step 11 — about blurb
    public string? AboutBlurb { get; set; }

    // Identity confirmation (brief 2.2)
    public bool IsBusiness { get; set; }
    public string? Abn { get; set; }
    public string? AbnEntityName { get; set; }
    public DateTimeOffset? AbnValidatedAt { get; set; }

    public List<SponsorWishlist> Wishlists { get; set; } = new();

    /// <summary>Set when the final onboarding step (identity) is saved.</summary>
    public DateTimeOffset? OnboardingCompletedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
