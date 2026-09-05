namespace SponsorMyAthlete.Domain.Entities;

public class ProfileSave
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }
    public Guid SponsorUserId { get; set; }
    public DateTimeOffset SavedAt { get; set; }
}
