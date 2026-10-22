using SponsorMyAthlete.Application.AthleteProfiles;

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

    /// <summary>The sponsor's own profile exactly as athletes see it.</summary>
    Task<PublicSponsorProfileDto> GetPublicForUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// The sponsor on the other side of a thread. Athletes only get it once the sponsor has sent a
    /// message — until then sponsors stay anonymous (brief 5.4).
    /// </summary>
    Task<PublicSponsorProfileDto> GetPublicForThreadAsync(Guid viewerUserId, Guid threadId, CancellationToken ct = default);

    Task<IReadOnlyList<PublicAthleteProfileDto>> GetSavedAthletesAsync(Guid sponsorUserId, CancellationToken ct = default);
    Task SaveAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default);
    Task UnsaveAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default);

    /// <summary>Public, anonymized sponsor directory for athletes (brief parity with the athlete directory).</summary>
    Task<IReadOnlyList<PublicSponsorSummaryDto>> SearchDirectoryAsync(string? sport, string? state, CancellationToken ct = default);
    Task<PublicSponsorSummaryDto?> GetSummaryByIdAsync(Guid sponsorProfileId, CancellationToken ct = default);
}
