using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.Payments;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

public record ConfirmPaymentMethodRequest(string PaymentMethodId);

/// <summary>Both athletes and sponsors attach a Stripe payment method during onboarding (brief step 12 / 2.2).</summary>
[Route("api/payments")]
public class PaymentsController(IUserSyncService userSyncService, IPaymentSetupService paymentSetupService)
    : ApiControllerBase(userSyncService)
{
    [HttpPost("setup-intent")]
    public async Task<ActionResult<SetupIntentResult>> StartSetupIntent(CancellationToken ct) =>
        Ok(await paymentSetupService.StartSetupIntentAsync(await GetCurrentUserIdAsync(ct), ct));

    [HttpPost("confirm-payment-method")]
    public async Task<IActionResult> ConfirmPaymentMethod([FromBody] ConfirmPaymentMethodRequest request, CancellationToken ct)
    {
        await paymentSetupService.ConfirmPaymentMethodAsync(await GetCurrentUserIdAsync(ct), request.PaymentMethodId, ct);
        return NoContent();
    }
}
