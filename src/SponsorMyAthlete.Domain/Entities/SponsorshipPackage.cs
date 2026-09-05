namespace SponsorMyAthlete.Domain.Entities;

/// <summary>An offering an athlete lists for sponsors (brief step 10).</summary>
public class SponsorshipPackage
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
