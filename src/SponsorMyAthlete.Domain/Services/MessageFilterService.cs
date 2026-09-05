using System.Text.RegularExpressions;

namespace SponsorMyAthlete.Domain.Services;

public record MessageFilterResult(bool IsBlocked, string? Reason)
{
    public static readonly MessageFilterResult Allowed = new(false, null);
}

public interface IMessageFilter
{
    MessageFilterResult Evaluate(string body, bool dealCompleted);
}

/// <summary>
/// Brief 7a.1/7a.2 — walled-garden messaging. Blocks emails, AU phone numbers, references to
/// off-platform apps, canned "contact me" phrases, and external links (until a deal completes).
/// </summary>
public sealed partial class MessageFilterService : IMessageFilter
{
    public MessageFilterResult Evaluate(string body, bool dealCompleted)
    {
        if (string.IsNullOrWhiteSpace(body))
            return MessageFilterResult.Allowed;

        if (EmailRegex().IsMatch(body))
            return new MessageFilterResult(true, "email_address");

        if (AuPhoneRegex().IsMatch(body))
            return new MessageFilterResult(true, "phone_number");

        if (OffPlatformAppRegex().IsMatch(body))
            return new MessageFilterResult(true, "off_platform_app_reference");

        if (ContactPhraseRegex().IsMatch(body))
            return new MessageFilterResult(true, "off_platform_contact_phrase");

        if (!dealCompleted && ExternalLinkRegex().IsMatch(body))
            return new MessageFilterResult(true, "external_link");

        return MessageFilterResult.Allowed;
    }

    [GeneratedRegex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(\+?61|0)4\d{2}[\s-]?\d{3}[\s-]?\d{3}")]
    private static partial Regex AuPhoneRegex();

    [GeneratedRegex(@"\b(whatsapp|telegram|snapchat|wechat|signal)\b", RegexOptions.IgnoreCase)]
    private static partial Regex OffPlatformAppRegex();

    [GeneratedRegex(@"\b(call me|my number is|email me|dm me|reach me at|contact me directly)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ContactPhraseRegex();

    [GeneratedRegex(@"(https?://|www\.)\S+", RegexOptions.IgnoreCase)]
    private static partial Regex ExternalLinkRegex();
}
