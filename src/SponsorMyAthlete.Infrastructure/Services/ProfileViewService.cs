using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.ProfileViews;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class ProfileViewService(AppDbContext db) : IProfileViewService
{
    private const string DefaultTimeZone = "Australia/Sydney";

    public async Task RecordAsync(Guid athleteProfileId, Guid? viewerUserId, string viewerKey, CancellationToken ct = default)
    {
        var profile = await db.AthleteProfiles
            .Where(a => a.Id == athleteProfileId && a.VerificationStatus == VerificationStatus.Published)
            .Select(a => new { a.UserId })
            .SingleOrDefaultAsync(ct);
        if (profile is null || profile.UserId == viewerUserId)
            return;

        UserRole? viewerRole = viewerUserId is null
            ? null
            : await db.Users.Where(u => u.Id == viewerUserId).Select(u => u.Role).SingleOrDefaultAsync(ct);
        if (viewerRole == UserRole.Admin)
            return;

        var now = DateTimeOffset.UtcNow;
        var windowStart = now - ProfileViewStats.DedupeWindow;
        var seenRecently = await db.ProfileViews.AnyAsync(
            v => v.AthleteProfileId == athleteProfileId && v.ViewerKey == viewerKey && v.ViewedAt >= windowStart, ct);
        if (seenRecently)
            return;

        db.ProfileViews.Add(new ProfileView
        {
            Id = Guid.NewGuid(),
            AthleteProfileId = athleteProfileId,
            ViewerSponsorUserId = viewerRole == UserRole.Sponsor ? viewerUserId : null,
            ViewerKey = viewerKey,
            ViewedAt = now,
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<ProfileViewStatsDto> GetStatsForAthleteAsync(Guid athleteUserId, ViewPeriod period, string? timeZone, CancellationToken ct = default)
    {
        var profile = await db.AthleteProfiles
            .Where(a => a.UserId == athleteUserId)
            .Select(a => new { a.Id, a.CreatedAt })
            .SingleOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException("Athlete profile not found.");

        var tz = ResolveTimeZone(timeZone);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz).DateTime);
        var days = ProfileViewStats.DaysIn(period);
        var from = days is { } d
            ? today.AddDays(1 - d)
            : DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(profile.CreatedAt, tz).DateTime);
        // Fetch the previous window too so the change-vs-previous figure needs no second query.
        var fetchFrom = days is { } n ? from.AddDays(-n) : from;
        var fetchFromUtc = ProfileViewStats.StartOfDayUtc(fetchFrom, tz);

        var views = await db.ProfileViews
            .Where(v => v.AthleteProfileId == profile.Id && v.ViewedAt >= fetchFromUtc)
            .Select(v => new { v.ViewedAt, IsSponsor = v.ViewerSponsorUserId != null })
            .ToListAsync(ct);

        var periodStartUtc = ProfileViewStats.StartOfDayUtc(from, tz);
        var current = views.Where(v => v.ViewedAt >= periodStartUtc).ToList();
        var daily = ProfileViewStats.BucketByDay(current.Select(v => v.ViewedAt), from, today, tz);

        return new ProfileViewStatsDto(
            period,
            current.Count,
            days is null ? null : views.Count - current.Count,
            current.Count(v => v.IsSponsor),
            daily.Select(x => new DailyViewCountDto(x.Date, x.Count)).ToList());
    }

    private static TimeZoneInfo ResolveTimeZone(string? timeZone) =>
        !string.IsNullOrWhiteSpace(timeZone) && TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out var tz)
            ? tz
            : TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZone);
}
