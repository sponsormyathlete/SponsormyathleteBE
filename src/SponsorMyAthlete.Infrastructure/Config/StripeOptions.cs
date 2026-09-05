namespace SponsorMyAthlete.Infrastructure.Config;

public class StripeOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>Stripe Price id for the $5 AUD/month athlete subscription (brief 2.1).</summary>
    public string AthleteMonthlyPriceId { get; set; } = string.Empty;
    public int TrialPeriodDays { get; set; } = 30;
}
