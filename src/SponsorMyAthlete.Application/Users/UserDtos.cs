using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Application.Users;

public record UserDto(Guid Id, string Email, UserRole? Role, bool IsFlagged, bool HasPaymentMethod, bool OnboardingComplete);

public interface IUserSyncService
{
    /// <param name="grantAdmin">True only when the email is Auth0-verified and listed in Admin:Emails.</param>
    Task<UserDto> EnsureUserAsync(string auth0Sub, string email, bool grantAdmin, CancellationToken ct = default);
    Task<UserDto> SetRoleAsync(string auth0Sub, UserRole role, CancellationToken ct = default);
    Task<UserDto?> GetByAuth0SubAsync(string auth0Sub, CancellationToken ct = default);
}
