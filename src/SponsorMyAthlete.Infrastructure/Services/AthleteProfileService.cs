using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class AthleteProfileService(AppDbContext db, IStripeService stripeService) : IAthleteProfileService
{
    public async Task<AthleteProfileDto> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        return ToDto(profile);
    }

    public async Task<AthleteProfileDto?> GetPublishedByIdAsync(Guid athleteProfileId, CancellationToken ct = default)
    {
        var profile = await Query()
            .SingleOrDefaultAsync(a => a.Id == athleteProfileId && a.VerificationStatus == VerificationStatus.Published, ct);
        return profile is null ? null : ToDto(profile);
    }

    public async Task<IReadOnlyList<AthleteProfileDto>> SearchDirectoryAsync(string? sport, string? state, CompetitiveLevel? competitiveLevel, CancellationToken ct = default)
    {
        var query = Query().Where(a => a.VerificationStatus == VerificationStatus.Published);
        if (!string.IsNullOrWhiteSpace(sport))
            query = query.Where(a => a.Sport == sport);
        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(a => a.State == state);
        if (competitiveLevel is not null)
            query = query.Where(a => a.CompetitiveLevel == competitiveLevel);

        var profiles = await query.ToListAsync(ct);
        return profiles.Select(ToDto).ToList();
    }

    public async Task<AthleteProfileDto> UpdateBasicsAsync(Guid userId, UpdateBasicsRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.Sport = request.Sport;
        profile.CompetitiveLevel = request.CompetitiveLevel;
        profile.Organisation = request.Organisation;
        profile.State = request.State;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> UpdatePersonalDetailsAsync(Guid userId, UpdatePersonalDetailsRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender;
        profile.WeightClass = request.WeightClass;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> UpdateBioAsync(Guid userId, UpdateBioRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.Bio = request.Bio;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> UpdateAccomplishmentsAsync(Guid userId, UpdateAccomplishmentsRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        db.AthleteAccomplishments.RemoveRange(profile.Accomplishments);
        profile.Accomplishments = request.Accomplishments
            .Select(a => new AthleteAccomplishment { Id = Guid.NewGuid(), AthleteProfileId = profile.Id, Description = a.Description, Year = a.Year })
            .ToList();
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> AddPhotoAsync(Guid userId, AddPhotoRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        if (request.IsProfilePhoto)
            foreach (var existing in profile.Photos)
                existing.IsProfilePhoto = false;

        profile.Photos.Add(new AthletePhoto
        {
            Id = Guid.NewGuid(),
            AthleteProfileId = profile.Id,
            Url = request.Url,
            IsProfilePhoto = request.IsProfilePhoto,
            UploadedAt = DateTimeOffset.UtcNow,
        });
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> RemovePhotoAsync(Guid userId, Guid photoId, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        var photo = profile.Photos.SingleOrDefault(p => p.Id == photoId)
            ?? throw new KeyNotFoundException("Photo not found.");
        profile.Photos.Remove(photo);
        db.AthletePhotos.Remove(photo);
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> UpdateSocialMediaAsync(Guid userId, UpdateSocialMediaRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        profile.InstagramHandle = request.InstagramHandle;
        profile.FacebookHandle = request.FacebookHandle;
        profile.FollowerCount = request.FollowerCount;
        profile.AverageReach = request.AverageReach;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> UpdateSponsorshipPackagesAsync(Guid userId, UpdateSponsorshipPackagesRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        db.SponsorshipPackages.RemoveRange(profile.SponsorshipPackages);
        profile.SponsorshipPackages = request.Packages
            .Select(p => new SponsorshipPackage { Id = Guid.NewGuid(), AthleteProfileId = profile.Id, Title = p.Title, Description = p.Description, CreatedAt = DateTimeOffset.UtcNow })
            .ToList();
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> SubmitVerificationAsync(Guid userId, SubmitVerificationRequest request, CancellationToken ct = default)
    {
        var profile = await GetOrCreateEntityAsync(userId, ct);
        if (string.IsNullOrWhiteSpace(profile.Sport) || string.IsNullOrWhiteSpace(profile.Bio) || profile.DateOfBirth is null)
            throw new InvalidOperationException("Complete sport, personal details, and bio before submitting for verification.");

        profile.VerificationReference = request.Reference;
        profile.VerificationStatus = VerificationStatus.PendingReview;
        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<IReadOnlyList<AthleteProfileDto>> GetPendingVerificationsAsync(CancellationToken ct = default)
    {
        var profiles = await Query().Where(a => a.VerificationStatus == VerificationStatus.PendingReview).ToListAsync(ct);
        return profiles.Select(ToDto).ToList();
    }

    public async Task<AthleteProfileDto> ApproveVerificationAsync(Guid athleteProfileId, ReviewVerificationRequest request, CancellationToken ct = default)
    {
        var profile = await Query().SingleAsync(a => a.Id == athleteProfileId, ct);
        profile.VerificationStatus = VerificationStatus.Published;
        profile.VerificationAdminNotes = request.AdminNotes;
        profile.VerifiedAt = DateTimeOffset.UtcNow;

        var user = await db.Users.SingleAsync(u => u.Id == profile.UserId, ct);
        if (!string.IsNullOrEmpty(user.StripeCustomerId))
        {
            var subscriptionId = await stripeService.StartAthleteSubscriptionAsync(user.StripeCustomerId, ct);
            profile.StripeSubscriptionId = subscriptionId;
            profile.SubscriptionStatus = SubscriptionStatus.Trialing;
        }

        return await SaveAndReturnAsync(profile, ct);
    }

    public async Task<AthleteProfileDto> RejectVerificationAsync(Guid athleteProfileId, ReviewVerificationRequest request, CancellationToken ct = default)
    {
        var profile = await Query().SingleAsync(a => a.Id == athleteProfileId, ct);
        profile.VerificationStatus = VerificationStatus.Rejected;
        profile.VerificationAdminNotes = request.AdminNotes;
        return await SaveAndReturnAsync(profile, ct);
    }

    private IQueryable<AthleteProfile> Query() =>
        db.AthleteProfiles
            .Include(a => a.Accomplishments)
            .Include(a => a.Photos)
            .Include(a => a.SponsorshipPackages);

    private async Task<AthleteProfile> GetOrCreateEntityAsync(Guid userId, CancellationToken ct)
    {
        var profile = await Query().SingleOrDefaultAsync(a => a.UserId == userId, ct);
        if (profile is not null)
            return profile;

        profile = new AthleteProfile { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTimeOffset.UtcNow };
        db.AthleteProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
        return profile;
    }

    private async Task<AthleteProfileDto> SaveAndReturnAsync(AthleteProfile profile, CancellationToken ct)
    {
        profile.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return ToDto(profile);
    }

    private static AthleteProfileDto ToDto(AthleteProfile p)
    {
        var strength = ProfileStrengthCalculator.Calculate(p);
        return new AthleteProfileDto(
            p.Id, p.UserId, p.Sport, p.CompetitiveLevel, p.Organisation, p.State,
            p.DateOfBirth, p.Gender, p.WeightClass, p.Bio,
            p.Accomplishments.Select(a => new AccomplishmentDto(a.Id, a.Description, a.Year)).ToList(),
            p.Photos.Select(ph => new AthletePhotoDto(ph.Id, ph.Url, ph.IsProfilePhoto)).ToList(),
            p.InstagramHandle, p.FacebookHandle, p.FollowerCount, p.AverageReach,
            p.SponsorshipPackages.Select(sp => new SponsorshipPackageDto(sp.Id, sp.Title, sp.Description)).ToList(),
            p.VerificationReference, p.VerificationStatus, p.VerificationAdminNotes, p.SubscriptionStatus,
            strength.Score
        );
    }
}
