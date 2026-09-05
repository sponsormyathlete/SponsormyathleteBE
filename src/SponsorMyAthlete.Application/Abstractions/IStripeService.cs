namespace SponsorMyAthlete.Application.Abstractions;

public record SetupIntentResult(string ClientSecret, string SetupIntentId);

public record PaymentLinkResult(string PaymentLinkId, string Url);

/// <summary>
/// Stripe integration surface. Athlete subscriptions ($5/mo, 30-day trial) and sponsor payment
/// links (10% cash / 5% non-cash fee) per brief sections 2.1/2.2/3.3. Real Stripe.net calls —
/// requires SecretKey/WebhookSecret/AthleteMonthlyPriceId configured under "Stripe" before use.
/// </summary>
public interface IStripeService
{
    Task<string> CreateCustomerAsync(string email, CancellationToken ct = default);

    /// <summary>Used at registration so both athletes and sponsors have a payment method on file.</summary>
    Task<SetupIntentResult> CreateSetupIntentAsync(string stripeCustomerId, CancellationToken ct = default);

    Task<bool> HasDefaultPaymentMethodAsync(string stripeCustomerId, CancellationToken ct = default);

    Task SetDefaultPaymentMethodAsync(string stripeCustomerId, string paymentMethodId, CancellationToken ct = default);

    /// <summary>Starts the athlete's $5/mo subscription once verification is approved (brief 2.1).</summary>
    Task<string> StartAthleteSubscriptionAsync(string stripeCustomerId, CancellationToken ct = default);

    /// <summary>Generates the sponsor-facing payment link for a concierge deal (brief 3.3 step 5).</summary>
    Task<PaymentLinkResult> CreateDealPaymentLinkAsync(string description, decimal totalAmountAud, CancellationToken ct = default);

    /// <summary>Verifies the Stripe-Signature header and returns the parsed event (as <c>Stripe.Event</c>).</summary>
    object ConstructWebhookEvent(string requestBody, string stripeSignatureHeader);
}
