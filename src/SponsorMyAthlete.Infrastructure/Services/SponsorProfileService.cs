using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.SponsorProfiles;
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
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.IsBusiness = request.IsBusiness;

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
        p.SponsorshipType, p.RelationshipStyle, p.AboutBlurb, p.IsBusiness, p.Abn, p.AbnEntityName, p.AbnValidatedAt,
        p.Wishlists.Select(w => new SponsorWishlistDto(w.Id, w.Title, w.Description)).ToList()
    );
}
