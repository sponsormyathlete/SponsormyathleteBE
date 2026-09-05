namespace SponsorMyAthlete.Domain.Entities;

public class AthletePhoto
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }
    public required string Url { get; set; }
    public bool IsProfilePhoto { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}
