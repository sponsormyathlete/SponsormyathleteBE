namespace SponsorMyAthlete.Domain.Entities;

public class AthleteAccomplishment
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }
    public required string Description { get; set; }
    public int? Year { get; set; }
}
