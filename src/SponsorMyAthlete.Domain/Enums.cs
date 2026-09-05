namespace SponsorMyAthlete.Domain;

public enum UserRole
{
    Athlete,
    Sponsor,
    Admin
}

public enum VerificationStatus
{
    Draft,
    PendingReview,
    Published,
    Rejected
}

public enum CompetitiveLevel
{
    Amateur,
    SemiPro,
    Professional
}

public enum SponsorshipType
{
    Cash,
    Product,
    Services,
    Mixed
}

public enum RelationshipStyle
{
    Transactional,
    Collaborative,
    Mentor
}

public enum EventAttendancePreference
{
    Yes,
    Maybe,
    NotImportant
}

public enum ReachImportance
{
    FollowingSize,
    Engagement,
    NotImportant
}

public enum AthleteLevelPreference
{
    Amateur,
    SemiPro,
    Professional,
    All
}

public enum DealType
{
    Cash,
    NonCash
}

public enum DealStatus
{
    Proposed,
    Agreed,
    PaymentLinkSent,
    Paid,
    DeliverablesSubmitted,
    Completed,
    Cancelled
}

public enum SubscriptionStatus
{
    None,
    Trialing,
    Active,
    PastDue,
    Canceled
}
