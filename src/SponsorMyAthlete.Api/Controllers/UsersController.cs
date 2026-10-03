using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Api.Auth;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Api.Controllers;

public record SyncUserRequest(string? Email);
public record SetRoleRequest(UserRole Role);

[Route("api/users")]
public class UsersController(
    IUserSyncService userSyncService,
    AuthMode authMode,
    Auth0UserInfoClient userInfoClient,
    IConfiguration configuration) : ApiControllerBase(userSyncService)
{
    [HttpPost("me/sync")]
    public async Task<ActionResult<UserDto>> Sync([FromBody] SyncUserRequest request, CancellationToken ct)
    {
        string email;
        var emailVerified = false;

        if (authMode.IsDevAuth)
        {
            email = request.Email ?? string.Empty;
        }
        else
        {
            var token = Request.Headers.Authorization.ToString()["Bearer ".Length..].Trim();
            var userInfo = await userInfoClient.GetAsync(token, ct);
            email = userInfo.Email ?? string.Empty;
            emailVerified = userInfo.EmailVerified;
        }

        var adminEmails = configuration.GetSection("Admin:Emails").Get<string[]>() ?? [];
        var grantAdmin = emailVerified && adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);

        return Ok(await userSyncService.EnsureUserAsync(CurrentAuth0Sub, email, grantAdmin, ct));
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me(CancellationToken ct)
    {
        var user = await userSyncService.GetByAuth0SubAsync(CurrentAuth0Sub, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost("me/role")]
    public async Task<ActionResult<UserDto>> SetRole([FromBody] SetRoleRequest request, CancellationToken ct)
    {
        // Admin is only ever granted via Admin:Emails at sync time; self-selecting it is a
        // local-testing convenience that must not exist once real Auth0 is configured.
        if (request.Role == UserRole.Admin && !authMode.IsDevAuth)
            return Forbid();

        return Ok(await userSyncService.SetRoleAsync(CurrentAuth0Sub, request.Role, ct));
    }
}
