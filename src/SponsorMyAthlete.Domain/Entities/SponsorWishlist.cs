namespace SponsorMyAthlete.Domain.Entities;

/// <summary>What a sponsor is looking for from a deal (brief 3.1).</summary>
public class SponsorWishlist
{
    public Guid Id { get; set; }
    public Guid SponsorProfileId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
