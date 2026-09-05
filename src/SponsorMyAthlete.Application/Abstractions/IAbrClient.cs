namespace SponsorMyAthlete.Application.Abstractions;

public record AbnLookupResult(bool IsValid, string? EntityName, string? Reason);

/// <summary>
/// ABR ABN Lookup integration (brief 2.2/3.1). Requires an ABR web services GUID configured
/// under "Abr:Guid" before live lookups will succeed; caller should local-validate the ABN
/// checksum (<see cref="SponsorMyAthlete.Domain.Services.AbnValidator"/>) first.
/// </summary>
public interface IAbrClient
{
    Task<AbnLookupResult> LookupAsync(string abn, CancellationToken ct = default);
}
