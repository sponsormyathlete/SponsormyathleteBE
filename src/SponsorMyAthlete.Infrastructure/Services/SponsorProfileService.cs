using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.SponsorProfiles;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class SponsorProfileService(AppDbContext db, IAbrClient abrClient) : ISponsorProfileService
{
    public async Task<SponsorProfileDto> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        return ToDto(profile);
    }

    public async Task<SponsorProfileDto> UpdateInterestsAsync(Guid userId, UpdateSponsorInterestsRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.SportsOfInterest = request.SportsOfInterest;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateBudgetAsync(Guid userId, UpdateSponsorBudgetRequest request, CancellationToken ct = default)
    {
        if (request.BudgetMin < 0 || request.BudgetMax < request.BudgetMin)
            throw new InvalidOperationException("The upper budget needs to be at least the starting amount.");

        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.BudgetMin = request.BudgetMin;
        profile.BudgetMax = request.BudgetMax;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateAthleteCountAsync(Guid userId, UpdateSponsorAthleteCountRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.AthleteCountMin = request.AthleteCountMin;
        profile.AthleteCountMax = request.AthleteCountMax;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateDeliverablesAsync(Guid userId, UpdateSponsorDeliverablesRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.KeyDeliverablesWanted = request.KeyDeliverablesWanted;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateLocationAsync(Guid userId, UpdateSponsorLocationRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.State = request.State;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateEventPreferenceAsync(Guid userId, UpdateSponsorEventPrefRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.AttendEvents = request.AttendEvents;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateAthleteLevelPreferenceAsync(Guid userId, UpdateSponsorAthleteLevelRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.AthleteLevelPreference = request.AthleteLevelPreference;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateReachImportanceAsync(Guid userId, UpdateSponsorReachImportanceRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.ReachImportance = request.ReachImportance;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateSponsorshipTypeAsync(Guid userId, UpdateSponsorTypeRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.SponsorshipType = request.SponsorshipType;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateRelationshipStyleAsync(Guid userId, UpdateSponsorRelationshipStyleRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.RelationshipStyle = request.RelationshipStyle;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateAboutAsync(Guid userId, UpdateSponsorAboutRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.AboutBlurb = request.AboutBlurb;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateIdentityAsync(Guid userId, UpdateSponsorIdentityRequest request, CancellationToken ct = default)
    {
        // Businesses can fall back to their ABR entity name; individuals have nothing else to show.
        if (!request.IsBusiness && string.IsNullOrWhiteSpace(request.DisplayName))
            throw new InvalidOperationException("Add the name you want athletes to see.");

        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.IsBusiness = request.IsBusiness;
        profile.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();

        if (request.IsBusiness)
        {
            if (!AbnValidator.IsValidFormat(request.Abn))
                throw new InvalidOperationException("That ABN isn't valid. Check the 11 digits and try again.");

            var lookup = await abrClient.LookupAsync(request.Abn!, ct);
            profile.Abn = request.Abn;
            profile.AbnEntityName = lookup.EntityName;
            profile.AbnValidatedAt = lookup.IsValid ? DateTimeOffset.UtcNow : null;
        }
        else
        {
            profile.Abn = null;
            profile.AbnEntityName = null;
            profile.AbnValidatedAt = null;
        }

        profile.OnboardingCompletedAt ??= DateTimeOffset.UtcNow;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<SponsorProfileDto> UpdateWishlistsAsync(Guid userId, UpdateSponsorWishlistsRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        db.SponsorWishlists.RemoveRange(profile.Wishlists);
        profile.Wishlists = request.Wishlists
            .Select(w => new SponsorWishlist { Id = Guid.NewGuid(), SponsorProfileId = profile.Id, Title = w.Title, Description = w.Description, CreatedAt = DateTimeOffset.UtcNow })
            .ToList();
        // Explicit Add: see AthleteProfileService.UpdateAccomplishmentsAsync.
        db.SponsorWishlists.AddRange(profile.Wishlists);
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<PublicSponsorProfileDto> GetPublicForUserAsync(Guid userId, CancellationToken ct = default) =>
        ToPublicDto(await GetOrCreateEntityAsync(userId, ct));

    public async Task<PublicSponsorProfileDto> GetPublicForThreadAsync(Guid viewerUserId, Guid threadId, CancellationToken ct = default)
    {
        var thread = await db.MessageThreads.SingleOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new KeyNotFoundException("Thread not found.");
        if (thread.AthleteUserId != viewerUserId && thread.SponsorUserId != viewerUserId)
            throw new UnauthorizedAccessException("You are not a party to this thread.");

        if (thread.AthleteUserId == viewerUserId && !await SponsorHasMessagedAsync(db, thread, ct))
            throw new UnauthorizedAccessException("This sponsor's profile becomes visible once they send you a message.");

        var profile = await db.SponsorProfiles.Include(s => s.Wishlists).SingleOrDefaultAsync(s => s.UserId == thread.SponsorUserId, ct)
            ?? throw new KeyNotFoundException("This sponsor hasn't set up a profile yet.");
        return ToPublicDto(profile);
    }

    public async Task<IReadOnlyList<PublicAthleteProfileDto>> GetSavedAthletesAsync(Guid sponsorUserId, CancellationToken ct = default)
    {
        var savedIds = db.ProfileSaves.Where(s => s.SponsorUserId == sponsorUserId).Select(s => s.AthleteProfileId);
        // Athletes who've since been unpublished drop out of the list but the save is kept.
        var profiles = await db.AthleteProfiles
            .Include(a => a.Accomplishments)
            .Include(a => a.Photos)
            .Include(a => a.SponsorshipPackages)
            .Where(a => savedIds.Contains(a.Id) && a.VerificationStatus == VerificationStatus.Published)
            .ToListAsync(ct);
        return profiles.Select(AthleteProfileService.ToPublicDto).ToList();
    }

    public async Task SaveAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default)
    {
        await EnsureSponsorAsync(sponsorUserId, ct);
        if (!await db.AthleteProfiles.AnyAsync(a => a.Id == athleteProfileId && a.VerificationStatus == VerificationStatus.Published, ct))
            throw new KeyNotFoundException("Athlete profile not found.");
        if (await db.ProfileSaves.AnyAsync(s => s.SponsorUserId == sponsorUserId && s.AthleteProfileId == athleteProfileId, ct))
            return;

        db.ProfileSaves.Add(new ProfileSave
        {
            Id = Guid.NewGuid(),
            AthleteProfileId = athleteProfileId,
            SponsorUserId = sponsorUserId,
            SavedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task UnsaveAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default)
    {
        var save = await db.ProfileSaves.SingleOrDefaultAsync(s => s.SponsorUserId == sponsorUserId && s.AthleteProfileId == athleteProfileId, ct);
        if (save is null)
            return;
        db.ProfileSaves.Remove(save);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PublicSponsorSummaryDto>> SearchDirectoryAsync(string? sport, string? state, CancellationToken ct = default)
    {
        var query = db.SponsorProfiles.Where(s => s.OnboardingCompletedAt != null);
        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(s => s.State == state);

        var profiles = await query.ToListAsync(ct);
        if (!string.IsNullOrWhiteSpace(sport))
            profiles = profiles.Where(s => s.SportsOfInterest.Contains(sport)).ToList();

        return profiles.Select(ToSummaryDto).ToList();
    }

    public async Task<PublicSponsorSummaryDto?> GetSummaryByIdAsync(Guid sponsorProfileId, CancellationToken ct = default)
    {
        var profile = await db.SponsorProfiles.SingleOrDefaultAsync(s => s.Id == sponsorProfileId && s.OnboardingCompletedAt != null, ct);
        return profile is null ? null : ToSummaryDto(profile);
    }

    private static PublicSponsorSummaryDto ToSummaryDto(SponsorProfile p) => new(
        p.Id, p.IsBusiness, p.SportsOfInterest, p.BudgetMin, p.BudgetMax, p.State,
        p.AthleteLevelPreference, p.SponsorshipType, p.RelationshipStyle, p.KeyDeliverablesWanted, p.AboutBlurb
    );

    internal static Task<bool> SponsorHasMessagedAsync(AppDbContext db, MessageThread thread, CancellationToken ct) =>
        db.Messages.AnyAsync(m => m.ThreadId == thread.Id && m.SenderUserId == thread.SponsorUserId, ct);

    internal static string PublicNameFor(SponsorProfile p) =>
        !string.IsNullOrWhiteSpace(p.DisplayName) ? p.DisplayName
        : !string.IsNullOrWhiteSpace(p.AbnEntityName) ? p.AbnEntityName
        : "Sponsor";

    private async Task EnsureSponsorAsync(Guid userId, CancellationToken ct)
    {
        var role = await db.Users.Where(u => u.Id == userId).Select(u => u.Role).SingleAsync(ct);
        if (role != UserRole.Sponsor)
            throw new UnauthorizedAccessException("Only sponsors can save athletes.");
    }

    private async Task<SponsorProfile> GetOrCreateEntityAsync(Guid userId, CancellationToken ct)
    {
        var profile = await db.SponsorProfiles.Include(s => s.Wishlists).SingleOrDefaultAsync(s => s.UserId == userId, ct);
        if (profile is not null)
            return profile;

        profile = new SponsorProfile { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTimeOffset.UtcNow };
        db.SponsorProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
        return profile;
    }

    private async Task<SponsorProfileDto> SaveAndReturnAsync(SponsorProfile profile, CancellationToken ct)
    {
        profile.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return ToDto(profile);
    }

    private static SponsorProfileDto ToDto(SponsorProfile p) => new(
        p.Id, p.UserId, p.SportsOfInterest, p.BudgetMin, p.BudgetMax, p.AthleteCountMin, p.AthleteCountMax,
        p.KeyDeliverablesWanted, p.State, p.AttendEvents, p.AthleteLevelPreference, p.ReachImportance,
        p.SponsorshipType, p.RelationshipStyle, p.AboutBlurb, p.DisplayName, p.IsBusiness, p.Abn, p.AbnEntityName, p.AbnValidatedAt,
        p.Wishlists.Select(w => new SponsorWishlistDto(w.Id, w.Title, w.Description)).ToList()
    );

    private static PublicSponsorProfileDto ToPublicDto(SponsorProfile p) => new(
        p.Id, PublicNameFor(p), p.IsBusiness, p.IsBusiness && p.AbnValidatedAt is not null,
        p.SportsOfInterest, p.BudgetMin, p.BudgetMax, p.AthleteCountMin, p.AthleteCountMax,
        p.KeyDeliverablesWanted, p.State, p.AttendEvents, p.AthleteLevelPreference, p.ReachImportance,
        p.SponsorshipType, p.RelationshipStyle, p.AboutBlurb,
        p.Wishlists.Select(w => new SponsorWishlistDto(w.Id, w.Title, w.Description)).ToList()
    );
}
