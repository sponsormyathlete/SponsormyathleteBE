namespace SponsorMyAthlete.Domain.Entities;

public class AdminFlag
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Resolved { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}
