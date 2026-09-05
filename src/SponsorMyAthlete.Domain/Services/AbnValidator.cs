using System.Text.RegularExpressions;

namespace SponsorMyAthlete.Domain.Services;

/// <summary>
/// Local ABN checksum validation (Australian Business Register algorithm), run before ever
/// calling the ABR API — cheap way to reject obviously malformed ABNs (brief 2.2 / 3.1).
/// </summary>
public static partial class AbnValidator
{
    private static readonly int[] Weights = { 10, 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };

    public static bool IsValidFormat(string? abn)
    {
        if (string.IsNullOrWhiteSpace(abn))
            return false;

        var digits = DigitsOnly().Replace(abn, string.Empty);
        if (digits.Length != 11)
            return false;

        var sum = 0;
        for (var i = 0; i < 11; i++)
        {
            var digit = digits[i] - '0';
            if (i == 0)
                digit -= 1;
            sum += digit * Weights[i];
        }

        return sum % 89 == 0;
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex DigitsOnly();
}
