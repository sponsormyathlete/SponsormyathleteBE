using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Infrastructure.Config;

namespace SponsorMyAthlete.Infrastructure.Services;

/// <summary>
/// ABR ABN Lookup JSON endpoint client (brief 2.2/3.1). Needs Abr:Guid configured — register
/// free at https://abr.business.gov.au/Tools/WebServices. Response shape is confirmed against
/// ABR's documented JSON contract; verify field names against a live response once the GUID
/// is supplied, since ABR wraps the payload in a JSONP-style callback.
/// </summary>
public partial class AbrClient(HttpClient httpClient, IOptions<AbrOptions> options) : IAbrClient
{
    private readonly AbrOptions _options = options.Value;

    public async Task<AbnLookupResult> LookupAsync(string abn, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_options.Guid))
            return new AbnLookupResult(false, null, "ABR GUID not configured");

        var url = $"https://abr.business.gov.au/json/AbnDetails.aspx?abn={Uri.EscapeDataString(abn)}&guid={Uri.EscapeDataString(_options.Guid)}";
        var raw = await httpClient.GetStringAsync(url, ct);

        var jsonText = JsonpWrapperRegex().Replace(raw, string.Empty).TrimEnd(')', ';');
        using var doc = JsonDocument.Parse(jsonText);
        var root = doc.RootElement;

        if (root.TryGetProperty("Message", out var message) && message.GetString() is { Length: > 0 } errorMessage)
            return new AbnLookupResult(false, null, errorMessage);

        var entityName = root.TryGetProperty("EntityName", out var name) ? name.GetString() : null;
        var abnStatus = root.TryGetProperty("AbnStatus", out var status) ? status.GetString() : null;
        var isValid = string.Equals(abnStatus, "Active", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(entityName);

        return new AbnLookupResult(isValid, entityName, isValid ? null : "ABN not active or not found");
    }

    [GeneratedRegex(@"^\s*\w+\(")]
    private static partial Regex JsonpWrapperRegex();
}
