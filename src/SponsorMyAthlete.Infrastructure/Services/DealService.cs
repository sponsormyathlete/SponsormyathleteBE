using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.Deals;
using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class DealService(AppDbContext db, IStripeService stripeService) : IDealService
{
    public async Task<DealDto> ProposeDealAsync(Guid sponsorUserId, Guid athleteProfileId, ProposeDealRequest request, CancellationToken ct = default)
    {
        var athleteProfile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.Id == athleteProfileId, ct)
            ?? throw new KeyNotFoundException("Athlete profile not found.");

        if (request.DeclaredValue <= 0)
            throw new InvalidOperationException("Enter the value you're offering before proposing a deal.");

        var thread = await db.MessageThreads
            .SingleOrDefaultAsync(t => t.AthleteUserId == athleteProfile.UserId && t.SponsorUserId == sponsorUserId, ct);
        if (thread is null)
        {
            thread = new MessageThread
            {
                Id = Guid.NewGuid(),
                AthleteUserId = athleteProfile.UserId,
                SponsorUserId = sponsorUserId,
                CreatedAt = DateTimeOffset.UtcNow,
                LastMessageAt = DateTimeOffset.UtcNow,
            };
            db.MessageThreads.Add(thread);
        }

        var deal = new Deal
        {
            Id = Guid.NewGuid(),
            AthleteUserId = athleteProfile.UserId,
            SponsorUserId = sponsorUserId,
            SponsorshipPackageId = request.SponsorshipPackageId,
            Title = request.Title,
            Note = request.Note,
            Type = request.Type,
            DeclaredValue = request.DeclaredValue,
            PlatformFeeAmount = DealFeeCalculator.CalculateFee(request.Type, request.DeclaredValue),
            Status = DealStatus.Proposed,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        db.Deals.Add(deal);

        thread.DealId = deal.Id;
        await db.SaveChangesAsync(ct);

        return await ToDtoAsync(deal, sponsorUserId, ct);
    }

    public async Task<IReadOnlyList<DealDto>> GetDealsForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var deals = await db.Deals
            .Where(d => d.AthleteUserId == userId || d.SponsorUserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        var result = new List<DealDto>();
        foreach (var deal in deals)
            result.Add(await ToDtoAsync(deal, userId, ct));
        return result;
    }

    public async Task<DealDto> GetDealAsync(Guid userId, Guid dealId, CancellationToken ct = default)
    {
        var deal = await GetOwnedDealAsync(userId, dealId, ct);
        return await ToDtoAsync(deal, userId, ct);
    }

    public async Task<DealDto> MarkDeliverablesSubmittedAsync(Guid userId, Guid dealId, CancellationToken ct = default)
    {
        var deal = await GetOwnedDealAsync(userId, dealId, ct);
        if (deal.AthleteUserId != userId)
            throw new UnauthorizedAccessException("Only the athlete can mark deliverables as submitted.");
        if (deal.Status != DealStatus.Paid)
            throw new InvalidOperationException("Deliverables can only be submitted once the sponsor has paid.");

        deal.Status = DealStatus.DeliverablesSubmitted;
        await db.SaveChangesAsync(ct);
        return await ToDtoAsync(deal, userId, ct);
    }

    public async Task<IReadOnlyList<AdminDealDto>> GetAllDealsAsync(CancellationToken ct = default)
    {
        var deals = await db.Deals.OrderByDescending(d => d.CreatedAt).ToListAsync(ct);
        var userIds = deals.SelectMany(d => new[] { d.AthleteUserId, d.SponsorUserId }).Distinct().ToList();
        var users = await db.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);
        var athleteProfiles = await db.AthleteProfiles.Where(a => userIds.Contains(a.UserId)).ToDictionaryAsync(a => a.UserId, ct);
        var sponsorProfiles = await db.SponsorProfiles.Where(s => userIds.Contains(s.UserId)).ToDictionaryAsync(s => s.UserId, ct);

        return deals.Select(d => new AdminDealDto(
            d.Id,
            d.AthleteUserId,
            users.GetValueOrDefault(d.AthleteUserId)?.Email ?? "unknown",
            athleteProfiles.GetValueOrDefault(d.AthleteUserId)?.DisplayName,
            d.SponsorUserId,
            users.GetValueOrDefault(d.SponsorUserId)?.Email ?? "unknown",
            sponsorProfiles.GetValueOrDefault(d.SponsorUserId)?.DisplayName,
            d.Title, d.Note, d.Type, d.DeclaredValue, d.PlatformFeeAmount, d.DeclaredValue + d.PlatformFeeAmount,
            d.Status, d.StripePaymentLinkUrl, d.AdminNotes, d.CreatedAt, d.PaidAt, d.CompletedAt
        )).ToList();
    }

    public async Task<AdminDealDto> AgreeAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default)
    {
        var deal = await db.Deals.SingleAsync(d => d.Id == dealId, ct);
        if (deal.Status != DealStatus.Proposed)
            throw new InvalidOperationException("Only a proposed deal can be marked agreed.");

        deal.Status = DealStatus.Agreed;
        deal.AdminNotes = request.AdminNotes;
        await db.SaveChangesAsync(ct);
        return await ToAdminDtoAsync(deal, ct);
    }

    public async Task<AdminDealDto> GeneratePaymentLinkAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await db.Deals.SingleAsync(d => d.Id == dealId, ct);
        if (deal.Status != DealStatus.Agreed)
            throw new InvalidOperationException("Agree the deal before generating a payment link.");

        var totalAmount = deal.DeclaredValue + deal.PlatformFeeAmount;
        var link = await stripeService.CreateDealPaymentLinkAsync(deal.Title, totalAmount, ct);

        deal.StripePaymentLinkId = link.PaymentLinkId;
        deal.StripePaymentLinkUrl = link.Url;
        deal.Status = DealStatus.PaymentLinkSent;
        await db.SaveChangesAsync(ct);
        return await ToAdminDtoAsync(deal, ct);
    }

    public async Task<AdminDealDto> CompleteAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default)
    {
        var deal = await db.Deals.SingleAsync(d => d.Id == dealId, ct);
        if (deal.Status is not (DealStatus.Paid or DealStatus.DeliverablesSubmitted))
            throw new InvalidOperationException("The deal must be paid before it can be marked complete.");

        deal.Status = DealStatus.Completed;
        deal.CompletedAt = DateTimeOffset.UtcNow;
        if (request.AdminNotes is not null)
            deal.AdminNotes = request.AdminNotes;
        await db.SaveChangesAsync(ct);
        return await ToAdminDtoAsync(deal, ct);
    }

    public async Task<AdminDealDto> CancelAsync(Guid dealId, AdminNoteRequest request, CancellationToken ct = default)
    {
        var deal = await db.Deals.SingleAsync(d => d.Id == dealId, ct);
        if (deal.Status is DealStatus.Completed or DealStatus.Cancelled)
            throw new InvalidOperationException("This deal is already finished.");

        deal.Status = DealStatus.Cancelled;
        deal.AdminNotes = request.AdminNotes;
        await db.SaveChangesAsync(ct);
        return await ToAdminDtoAsync(deal, ct);
    }

    public async Task MarkPaidByPaymentLinkIdAsync(string stripePaymentLinkId, CancellationToken ct = default)
    {
        var deal = await db.Deals.SingleOrDefaultAsync(d => d.StripePaymentLinkId == stripePaymentLinkId, ct);
        if (deal is null || deal.Status != DealStatus.PaymentLinkSent)
            return;

        deal.Status = DealStatus.Paid;
        deal.PaidAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    private async Task<Deal> GetOwnedDealAsync(Guid userId, Guid dealId, CancellationToken ct)
    {
        var deal = await db.Deals.SingleOrDefaultAsync(d => d.Id == dealId, ct)
            ?? throw new KeyNotFoundException("Deal not found.");
        if (deal.AthleteUserId != userId && deal.SponsorUserId != userId)
            throw new UnauthorizedAccessException("You are not a party to this deal.");
        return deal;
    }

    private async Task<DealDto> ToDtoAsync(Deal deal, Guid viewerUserId, CancellationToken ct)
    {
        string label;
        if (deal.AthleteUserId == viewerUserId)
        {
            var sponsorProfile = await db.SponsorProfiles.SingleOrDefaultAsync(s => s.UserId == deal.SponsorUserId, ct);
            label = sponsorProfile is null ? "Sponsor" : SponsorProfileService.PublicNameFor(sponsorProfile);
        }
        else
        {
            var athleteProfile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.UserId == deal.AthleteUserId, ct);
            label = athleteProfile is null ? "Athlete" : AthleteProfileService.PublicNameFor(athleteProfile);
        }

        return new DealDto(
            deal.Id, label, deal.Title, deal.Note, deal.Type, deal.DeclaredValue, deal.PlatformFeeAmount,
            deal.DeclaredValue + deal.PlatformFeeAmount, deal.Status, deal.StripePaymentLinkUrl,
            deal.CreatedAt, deal.PaidAt, deal.CompletedAt
        );
    }

    private async Task<AdminDealDto> ToAdminDtoAsync(Deal d, CancellationToken ct)
    {
        var athleteUser = await db.Users.SingleAsync(u => u.Id == d.AthleteUserId, ct);
        var sponsorUser = await db.Users.SingleAsync(u => u.Id == d.SponsorUserId, ct);
        var athleteProfile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.UserId == d.AthleteUserId, ct);
        var sponsorProfile = await db.SponsorProfiles.SingleOrDefaultAsync(s => s.UserId == d.SponsorUserId, ct);

        return new AdminDealDto(
            d.Id, d.AthleteUserId, athleteUser.Email, athleteProfile?.DisplayName,
            d.SponsorUserId, sponsorUser.Email, sponsorProfile?.DisplayName,
            d.Title, d.Note, d.Type, d.DeclaredValue, d.PlatformFeeAmount, d.DeclaredValue + d.PlatformFeeAmount,
            d.Status, d.StripePaymentLinkUrl, d.AdminNotes, d.CreatedAt, d.PaidAt, d.CompletedAt
        );
    }
}
