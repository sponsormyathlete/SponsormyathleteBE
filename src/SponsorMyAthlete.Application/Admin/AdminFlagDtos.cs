namespace SponsorMyAthlete.Application.Admin;

public record AdminFlagDto(Guid Id, Guid UserId, string UserEmail, string Reason, DateTimeOffset CreatedAt, bool Resolved);

public interface IAdminFlagService
{
    Task<IReadOnlyList<AdminFlagDto>> GetFlagsAsync(bool includeResolved, CancellationToken ct = default);
    Task ResolveFlagAsync(Guid flagId, CancellationToken ct = default);
}
