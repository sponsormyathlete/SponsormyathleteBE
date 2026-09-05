namespace SponsorMyAthlete.Domain.Entities;

/// <summary>Logged whenever the contact-detection filter blocks an outgoing message (brief 7a.2).</summary>
public class BlockedMessageAttempt
{
    public Guid Id { get; set; }
    public Guid? ThreadId { get; set; }
    public Guid SenderUserId { get; set; }
    public required string AttemptedBody { get; set; }
    public required string MatchedReason { get; set; }
    public DateTimeOffset AttemptedAt { get; set; }
}
