using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace SponsorMyAthlete.Api.Auth;

public record AuthMode(bool IsDevAuth);

public record Auth0UserInfo(
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("email_verified")] bool EmailVerified);

/// <summary>
/// Access tokens for a custom API audience don't carry the user's email, and an email sent by
/// the client can't be trusted. Calling Auth0's /userinfo with the caller's own token returns the
/// email Auth0 verified for that exact user.
/// </summary>
public class Auth0UserInfoClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<Auth0UserInfo> GetAsync(string bearerToken, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://{configuration["Auth0:Domain"]}/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Auth0UserInfo>(ct)
            ?? throw new InvalidOperationException("Auth0 /userinfo returned an empty body.");
    }
}
