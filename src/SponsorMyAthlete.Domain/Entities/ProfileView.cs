namespace SponsorMyAthlete.Domain.Entities;

public class ProfileView
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }
    public Guid? ViewerSponsorUserId { get; set; }
    public DateTimeOffset ViewedAt { get; set; }
}
