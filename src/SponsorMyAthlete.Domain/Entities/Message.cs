namespace SponsorMyAthlete.Domain.Entities;

/// <summary>An allowed, delivered message. Blocked attempts are never stored here — see <see cref="BlockedMessageAttempt"/>.</summary>
public class Message
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public Guid SenderUserId { get; set; }
    public required string Body { get; set; }
    public DateTimeOffset SentAt { get; set; }
}
