using SponsorMyAthlete.Domain.Services;
using Xunit;

namespace SponsorMyAthlete.Domain.Tests;

public class ProfileViewStatsTests
{
    private static readonly TimeZoneInfo Sydney = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");

    [Fact]
    public void BucketByDay_ZeroFillsEveryDayInRange()
    {
        var days = ProfileViewStats.BucketByDay([], new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 7), TimeZoneInfo.Utc);

        Assert.Equal(7, days.Count);
        Assert.All(days, d => Assert.Equal(0, d.Count));
        Assert.Equal(new DateOnly(2026, 10, 1), days[0].Date);
        Assert.Equal(new DateOnly(2026, 10, 7), days[^1].Date);
    }

    [Fact]
    public void BucketByDay_UsesViewerTimeZoneNotUtc()
    {
        // 20:00 UTC on 1 Oct is 07:00 on 2 Oct in Sydney (AEDT, UTC+11).
        var view = new DateTimeOffset(2026, 10, 1, 20, 0, 0, TimeSpan.Zero);

        var days = ProfileViewStats.BucketByDay([view], new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 2), Sydney);

        Assert.Equal(0, days[0].Count);
        Assert.Equal(1, days[1].Count);
    }

    [Fact]
    public void BucketByDay_IgnoresViewsOutsideRange()
    {
        var inside = new DateTimeOffset(2026, 10, 5, 12, 0, 0, TimeSpan.Zero);
        var before = new DateTimeOffset(2026, 9, 30, 12, 0, 0, TimeSpan.Zero);

        var days = ProfileViewStats.BucketByDay([inside, inside, before], new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 7), TimeZoneInfo.Utc);

        Assert.Equal(2, days.Sum(d => d.Count));
    }

    [Fact]
    public void StartOfDayUtc_ConvertsLocalMidnight()
    {
        var start = ProfileViewStats.StartOfDayUtc(new DateOnly(2026, 10, 13), Sydney);

        Assert.Equal(new DateTimeOffset(2026, 10, 12, 13, 0, 0, TimeSpan.Zero), start);
    }
}
