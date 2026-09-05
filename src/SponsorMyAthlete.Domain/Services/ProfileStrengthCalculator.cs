using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Domain.Services;

public record ProfileStrengthItem(string Key, string Label, int PointsAvailable, bool Complete);

public record ProfileStrengthResult(int Score, IReadOnlyList<ProfileStrengthItem> Items);

/// <summary>Brief 5.2 — weighted 0-100 completeness score with per-item points available.</summary>
public static class ProfileStrengthCalculator
{
    private const int MinBioWords = 100;
    private const int MinCompetitionPhotos = 3;
    private const int MinAccomplishments = 3;
    private const int RecencyWindowDays = 30;

    public static ProfileStrengthResult Calculate(AthleteProfile profile)
    {
        var hasProfilePhoto = profile.Photos.Any(p => p.IsProfilePhoto);
        var bioWordCount = string.IsNullOrWhiteSpace(profile.Bio)
            ? 0
            : profile.Bio.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;
        var hasSocialLinked = !string.IsNullOrWhiteSpace(profile.InstagramHandle) || !string.IsNullOrWhiteSpace(profile.FacebookHandle);
        var competitionPhotoCount = profile.Photos.Count(p => !p.IsProfilePhoto);
        var recentlyUpdated = profile.UpdatedAt is not null
            && profile.UpdatedAt.Value >= DateTimeOffset.UtcNow.AddDays(-RecencyWindowDays);

        var items = new List<ProfileStrengthItem>
        {
            new("profile_photo", "Profile photo added", 20, hasProfilePhoto),
            new("bio", $"Bio written at {MinBioWords}+ words", 15, bioWordCount >= MinBioWords),
            new("social_linked", "Instagram or Facebook linked", 15, hasSocialLinked),
            new("accomplishments", $"{MinAccomplishments}+ accomplishments listed", 10, profile.Accomplishments.Count >= MinAccomplishments),
            new("package", "At least one sponsorship package listed", 10, profile.SponsorshipPackages.Count >= 1),
            new("competition_photos", $"{MinCompetitionPhotos}+ competition photos uploaded", 10, competitionPhotoCount >= MinCompetitionPhotos),
            new("organisation", "Organisation or club listed", 10, !string.IsNullOrWhiteSpace(profile.Organisation)),
            new("recency", "Profile updated within last 30 days", 10, recentlyUpdated),
        };

        var score = items.Where(i => i.Complete).Sum(i => i.PointsAvailable);
        return new ProfileStrengthResult(score, items);
    }
}
