namespace SponsorMyAthlete.Application.SponsorProfiles;

public interface ISponsorProfileService
{
    Task<SponsorProfileDto> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default);

    Task<SponsorProfileDto> UpdateInterestsAsync(Guid userId, UpdateSponsorInterestsRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateBudgetAsync(Guid userId, UpdateSponsorBudgetRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateAthleteCountAsync(Guid userId, UpdateSponsorAthleteCountRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateDeliverablesAsync(Guid userId, UpdateSponsorDeliverablesRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateLocationAsync(Guid userId, UpdateSponsorLocationRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateEventPreferenceAsync(Guid userId, UpdateSponsorEventPrefRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateAthleteLevelPreferenceAsync(Guid userId, UpdateSponsorAthleteLevelRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateReachImportanceAsync(Guid userId, UpdateSponsorReachImportanceRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateSponsorshipTypeAsync(Guid userId, UpdateSponsorTypeRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateRelationshipStyleAsync(Guid userId, UpdateSponsorRelationshipStyleRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateAboutAsync(Guid userId, UpdateSponsorAboutRequest request, CancellationToken ct = default);

    /// <summary>Business sponsors: validates ABN checksum + ABR lookup (brief 2.2). Individual sponsors skip ABN.</summary>
    Task<SponsorProfileDto> UpdateIdentityAsync(Guid userId, UpdateSponsorIdentityRequest request, CancellationToken ct = default);
    Task<SponsorProfileDto> UpdateWishlistsAsync(Guid userId, UpdateSponsorWishlistsRequest request, CancellationToken ct = default);
}
