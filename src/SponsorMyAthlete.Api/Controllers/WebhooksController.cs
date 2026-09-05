using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Infrastructure.Persistence;
using Stripe;

namespace SponsorMyAthlete.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/webhooks/stripe")]
public class WebhooksController(IStripeService stripeService, AppDbContext db, ILogger<WebhooksController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        var body = await new StreamReader(Request.Body).ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        Event stripeEvent;
        try
        {
            stripeEvent = (Event)stripeService.ConstructWebhookEvent(body, signature);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Rejected Stripe webhook with invalid signature.");
            return BadRequest();
        }

        // Deal payment-link events (checkout/payment_intent) are wired up alongside the
        // concierge Deal flow (brief 3.3) — not yet built in this milestone.
        if (stripeEvent.Data.Object is Subscription subscription)
        {
            var profile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.StripeSubscriptionId == subscription.Id, ct);
            if (profile is not null)
            {
                profile.SubscriptionStatus = subscription.Status switch
                {
                    "trialing" => SubscriptionStatus.Trialing,
                    "active" => SubscriptionStatus.Active,
                    "past_due" => SubscriptionStatus.PastDue,
                    "canceled" => SubscriptionStatus.Canceled,
                    _ => profile.SubscriptionStatus,
                };
                await db.SaveChangesAsync(ct);
            }
        }

        return Ok();
    }
}
