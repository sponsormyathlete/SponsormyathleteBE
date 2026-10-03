using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SponsorMyAthlete.Api.Auth;

public static class DevAuth
{
    public const string Scheme = "DevAuth";
}

/// <summary>
/// Local-only stand-in for Auth0 JWT validation, wired up in Program.cs only when Auth0:Domain
/// is unset and the environment is Development. Treats the raw "Authorization: Bearer &lt;value&gt;"
/// value as the "sub" claim directly — no signature, no expiry — so the app can be clicked through
/// end-to-end before an Auth0 tenant exists. Never enabled outside Development.
/// </summary>
public class DevAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        var sub = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..].Trim()
            : string.Empty;

        if (string.IsNullOrEmpty(sub))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity([new Claim("sub", sub)], DevAuth.Scheme);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), DevAuth.Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
