namespace SponsorMyAthlete.Domain.Services;

public enum ViewPeriod
{
    Last7Days,
    Last30Days,
    Last90Days,
    AllTime
}

public record DailyViewCount(DateOnly Date, int Count);

/// <summary>Brief 5.1 — profile view counting rules and per-day bucketing for the metrics dashboard.</summary>
public static class ProfileViewStats
{
    /// <summary>Repeat views from the same viewer inside this window count once.</summary>
    public static readonly TimeSpan DedupeWindow = TimeSpan.FromMinutes(30);

    /// <summary>Number of days in a fixed period, or null for all time.</summary>
    public static int? DaysIn(ViewPeriod period) => period switch
    {
        ViewPeriod.Last7Days => 7,
        ViewPeriod.Last30Days => 30,
        ViewPeriod.Last90Days => 90,
        _ => null,
    };

    /// <summary>
    /// Counts views per calendar day in the viewer's time zone, returning every day from
    /// <paramref name="from"/> to <paramref name="to"/> inclusive (zero-filled) so the chart has no gaps.
    /// </summary>
    public static IReadOnlyList<DailyViewCount> BucketByDay(
        IEnumerable<DateTimeOffset> viewedAt, DateOnly from, DateOnly to, TimeZoneInfo timeZone)
    {
        var counts = viewedAt
            .Select(v => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(v, timeZone).DateTime))
            .Where(d => d >= from && d <= to)
            .GroupBy(d => d)
            .ToDictionary(g => g.Key, g => g.Count());

        var days = new List<DailyViewCount>();
        for (var day = from; day <= to; day = day.AddDays(1))
            days.Add(new DailyViewCount(day, counts.GetValueOrDefault(day)));
        return days;
    }

    /// <summary>The UTC instant at which a local calendar day starts in <paramref name="timeZone"/>.</summary>
    public static DateTimeOffset StartOfDayUtc(DateOnly day, TimeZoneInfo timeZone)
    {
        var localMidnight = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        // Midnight can fall in a DST gap in some zones; nudge forward to the first valid local time.
        while (timeZone.IsInvalidTime(localMidnight))
            localMidnight = localMidnight.AddMinutes(30);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(localMidnight, timeZone), TimeSpan.Zero);
    }
}
