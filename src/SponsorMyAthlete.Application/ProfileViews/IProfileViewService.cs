using SponsorMyAthlete.Domain.Services;

namespace SponsorMyAthlete.Application.ProfileViews;

public record DailyViewCountDto(DateOnly Date, int Count);

public record ProfileViewStatsDto(
    ViewPeriod Period,
    int Total,
    // Same-length window immediately before this one; null for all time.
    int? PreviousTotal,
    // Views by signed-in sponsors (the rest are anonymous visitors or other athletes).
    int SponsorViews,
    IReadOnlyList<DailyViewCountDto> Daily
);

public interface IProfileViewService
{
    /// <summary>
    /// Records a view of a published profile. Owners viewing themselves, admins, and repeat views by
    /// the same viewer within <see cref="ProfileViewStats.DedupeWindow"/> are not counted.
    /// </summary>
    Task RecordAsync(Guid athleteProfileId, Guid? viewerUserId, string viewerKey, CancellationToken ct = default);

    /// <param name="timeZone">IANA zone used for daily buckets; falls back to Australia/Sydney.</param>
    Task<ProfileViewStatsDto> GetStatsForAthleteAsync(Guid athleteUserId, ViewPeriod period, string? timeZone, CancellationToken ct = default);
}
