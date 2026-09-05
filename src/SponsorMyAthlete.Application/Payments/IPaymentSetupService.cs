using SponsorMyAthlete.Application.Abstractions;

namespace SponsorMyAthlete.Application.Payments;

/// <summary>
/// Brief step 12 (athlete) / section 2.2 (individual sponsor identity confirmation) — both
/// roles attach a Stripe payment method as part of onboarding, before any charge occurs.
/// </summary>
public interface IPaymentSetupService
{
    Task<SetupIntentResult> StartSetupIntentAsync(Guid userId, CancellationToken ct = default);
    Task ConfirmPaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken ct = default);
}
