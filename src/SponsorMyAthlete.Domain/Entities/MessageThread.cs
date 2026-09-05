namespace SponsorMyAthlete.Domain.Entities;

public class MessageThread
{
    public Guid Id { get; set; }
    public Guid AthleteUserId { get; set; }
    public Guid SponsorUserId { get; set; }
    public Guid? DealId { get; set; }
    public Deal? Deal { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastMessageAt { get; set; }

    public List<Message> Messages { get; set; } = new();
}
