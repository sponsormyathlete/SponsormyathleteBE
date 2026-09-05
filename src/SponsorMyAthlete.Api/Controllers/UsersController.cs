using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Api.Controllers;

public record SyncUserRequest(string Email);
public record SetRoleRequest(UserRole Role);

[Route("api/users")]
public class UsersController(IUserSyncService userSyncService) : ApiControllerBase(userSyncService)
{
    [HttpPost("me/sync")]
    public async Task<ActionResult<UserDto>> Sync([FromBody] SyncUserRequest request, CancellationToken ct)
    {
        var user = await userSyncService.EnsureUserAsync(CurrentAuth0Sub, request.Email, ct);
        return Ok(user);
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
        var user = await userSyncService.SetRoleAsync(CurrentAuth0Sub, request.Role, ct);
        return Ok(user);
    }
}
