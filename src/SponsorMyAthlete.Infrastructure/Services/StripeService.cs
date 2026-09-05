using Microsoft.Extensions.Options;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Infrastructure.Config;
using Stripe;

namespace SponsorMyAthlete.Infrastructure.Services;

/// <summary>
/// Real Stripe.net integration. Needs <see cref="StripeOptions"/> populated (SecretKey,
/// WebhookSecret, AthleteMonthlyPriceId) via configuration/user-secrets before use — see
/// appsettings.json "Stripe" section placeholders.
/// </summary>
public class StripeService(IOptions<StripeOptions> options) : IStripeService
{
    private readonly StripeOptions _options = options.Value;

    public async Task<string> CreateCustomerAsync(string email, CancellationToken ct = default)
    {
        var service = new CustomerService();
        var customer = await service.CreateAsync(new CustomerCreateOptions { Email = email }, cancellationToken: ct);
        return customer.Id;
    }

    public async Task<SetupIntentResult> CreateSetupIntentAsync(string stripeCustomerId, CancellationToken ct = default)
    {
        var service = new SetupIntentService();
        var setupIntent = await service.CreateAsync(new SetupIntentCreateOptions
        {
            Customer = stripeCustomerId,
            Usage = "off_session",
        }, cancellationToken: ct);

        return new SetupIntentResult(setupIntent.ClientSecret, setupIntent.Id);
    }

    public async Task<bool> HasDefaultPaymentMethodAsync(string stripeCustomerId, CancellationToken ct = default)
    {
        var service = new CustomerService();
        var customer = await service.GetAsync(stripeCustomerId, cancellationToken: ct);
        return !string.IsNullOrEmpty(customer.InvoiceSettings?.DefaultPaymentMethodId);
    }

    public async Task SetDefaultPaymentMethodAsync(string stripeCustomerId, string paymentMethodId, CancellationToken ct = default)
    {
        var paymentMethodService = new PaymentMethodService();
        await paymentMethodService.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions { Customer = stripeCustomerId }, cancellationToken: ct);

        var customerService = new CustomerService();
        await customerService.UpdateAsync(stripeCustomerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = paymentMethodId },
        }, cancellationToken: ct);
    }

    public async Task<string> StartAthleteSubscriptionAsync(string stripeCustomerId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_options.AthleteMonthlyPriceId))
            throw new InvalidOperationException("Stripe:AthleteMonthlyPriceId is not configured.");

        var service = new SubscriptionService();
        var subscription = await service.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = stripeCustomerId,
            Items = [new SubscriptionItemOptions { Price = _options.AthleteMonthlyPriceId }],
            TrialPeriodDays = _options.TrialPeriodDays,
        }, cancellationToken: ct);

        return subscription.Id;
    }

    public async Task<PaymentLinkResult> CreateDealPaymentLinkAsync(string description, decimal totalAmountAud, CancellationToken ct = default)
    {
        var priceService = new PriceService();
        var price = await priceService.CreateAsync(new PriceCreateOptions
        {
            Currency = "aud",
            UnitAmount = (long)(totalAmountAud * 100),
            ProductData = new PriceProductDataOptions { Name = description },
        }, cancellationToken: ct);

        var linkService = new PaymentLinkService();
        var link = await linkService.CreateAsync(new PaymentLinkCreateOptions
        {
            LineItems = [new PaymentLinkLineItemOptions { Price = price.Id, Quantity = 1 }],
        }, cancellationToken: ct);

        return new PaymentLinkResult(link.Id, link.Url);
    }

    public object ConstructWebhookEvent(string requestBody, string stripeSignatureHeader) =>
        EventUtility.ConstructEvent(requestBody, stripeSignatureHeader, _options.WebhookSecret);
}
