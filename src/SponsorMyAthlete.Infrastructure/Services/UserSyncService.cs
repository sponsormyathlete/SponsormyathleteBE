using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class UserSyncService(AppDbContext db) : IUserSyncService
{
    public async Task<UserDto> EnsureUserAsync(string auth0Sub, string email, bool grantAdmin, CancellationToken ct = default)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Auth0Sub == auth0Sub, ct);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Auth0Sub = auth0Sub,
                Email = email,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            db.Users.Add(user);
        }
        else
        {
            user.Email = email;
        }

        if (grantAdmin)
            user.Role = UserRole.Admin;

        await db.SaveChangesAsync(ct);
        return ToDto(user);
    }

    public async Task<UserDto> SetRoleAsync(string auth0Sub, UserRole role, CancellationToken ct = default)
    {
        var user = await db.Users.SingleAsync(u => u.Auth0Sub == auth0Sub, ct);
        if (user.Role is not null && user.Role != role)
            throw new InvalidOperationException("Role is already set and cannot be changed.");

        user.Role = role;
        await db.SaveChangesAsync(ct);
        return ToDto(user);
    }

    public async Task<UserDto?> GetByAuth0SubAsync(string auth0Sub, CancellationToken ct = default)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Auth0Sub == auth0Sub, ct);
        return user is null ? null : ToDto(user);
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.Role, user.IsFlagged, !string.IsNullOrEmpty(user.StripeDefaultPaymentMethodId));
}
