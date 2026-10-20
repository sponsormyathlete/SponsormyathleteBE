using System.Security.Cryptography;
using System.Text;

namespace SponsorMyAthlete.Api.Auth;

/// <summary>
/// Turns an anonymous visitor's IP + user agent into an opaque key for de-duplicating profile views.
/// The HMAC key is random per process, so stored keys can't be reversed or linked across restarts —
/// at worst a restart lets one repeat view through.
/// </summary>
public class ViewerKeyHasher
{
    private readonly byte[] _key = RandomNumberGenerator.GetBytes(32);

    public string ForUser(Guid userId) => $"u:{userId}";

    public string ForAnonymous(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var hash = HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes($"{ip}|{userAgent}"));
        return Convert.ToHexStringLower(hash);
    }
}
