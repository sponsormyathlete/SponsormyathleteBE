namespace SponsorMyAthlete.Domain.Entities;

public class ProfileView
{
    public Guid Id { get; set; }
    public Guid AthleteProfileId { get; set; }

    /// <summary>Set only when the viewer was a signed-in sponsor (brief 5.4 activity feed).</summary>
    public Guid? ViewerSponsorUserId { get; set; }

    /// <summary>
    /// Opaque per-viewer key used to de-duplicate repeat views — "u:{userId}" for signed-in users,
    /// otherwise a keyed hash of IP + user agent. Never the raw IP.
    /// </summary>
    public required string ViewerKey { get; set; }

    public DateTimeOffset ViewedAt { get; set; }
}
