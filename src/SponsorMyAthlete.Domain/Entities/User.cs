namespace SponsorMyAthlete.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Auth0Sub { get; set; }
    public required string Email { get; set; }
    public UserRole? Role { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeDefaultPaymentMethodId { get; set; }
    public bool IsFlagged { get; set; }
    public int BlockedAttemptCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public AthleteProfile? AthleteProfile { get; set; }
    public SponsorProfile? SponsorProfile { get; set; }
}
