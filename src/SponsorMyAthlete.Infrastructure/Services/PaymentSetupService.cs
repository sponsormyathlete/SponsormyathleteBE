using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.Payments;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class PaymentSetupService(AppDbContext db, IStripeService stripeService) : IPaymentSetupService
{
    public async Task<SetupIntentResult> StartSetupIntentAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await db.Users.SingleAsync(u => u.Id == userId, ct);
        if (string.IsNullOrEmpty(user.StripeCustomerId))
        {
            user.StripeCustomerId = await stripeService.CreateCustomerAsync(user.Email, ct);
            await db.SaveChangesAsync(ct);
        }

        return await stripeService.CreateSetupIntentAsync(user.StripeCustomerId, ct);
    }

    public async Task ConfirmPaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken ct = default)
    {
        var user = await db.Users.SingleAsync(u => u.Id == userId, ct);
        if (string.IsNullOrEmpty(user.StripeCustomerId))
            throw new InvalidOperationException("Setup intent must be started before confirming a payment method.");

        await stripeService.SetDefaultPaymentMethodAsync(user.StripeCustomerId, paymentMethodId, ct);
        user.StripeDefaultPaymentMethodId = paymentMethodId;
        await db.SaveChangesAsync(ct);
    }
}
