using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Application.AthleteProfiles;

public interface IAthleteProfileService
{
    Task<AthleteProfileDto> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default);
    Task<AthleteProfileDto?> GetPublishedByIdAsync(Guid athleteProfileId, CancellationToken ct = default);
    Task<IReadOnlyList<AthleteProfileDto>> SearchDirectoryAsync(string? sport, string? state, CompetitiveLevel? competitiveLevel, CancellationToken ct = default);

    Task<AthleteProfileDto> UpdateBasicsAsync(Guid userId, UpdateBasicsRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> UpdatePersonalDetailsAsync(Guid userId, UpdatePersonalDetailsRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> UpdateBioAsync(Guid userId, UpdateBioRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> UpdateAccomplishmentsAsync(Guid userId, UpdateAccomplishmentsRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> AddPhotoAsync(Guid userId, AddPhotoRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> RemovePhotoAsync(Guid userId, Guid photoId, CancellationToken ct = default);
    Task<AthleteProfileDto> UpdateSocialMediaAsync(Guid userId, UpdateSocialMediaRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> UpdateSponsorshipPackagesAsync(Guid userId, UpdateSponsorshipPackagesRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> SubmitVerificationAsync(Guid userId, SubmitVerificationRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<AthleteProfileDto>> GetPendingVerificationsAsync(CancellationToken ct = default);
    Task<AthleteProfileDto> ApproveVerificationAsync(Guid athleteProfileId, ReviewVerificationRequest request, CancellationToken ct = default);
    Task<AthleteProfileDto> RejectVerificationAsync(Guid athleteProfileId, ReviewVerificationRequest request, CancellationToken ct = default);
}
