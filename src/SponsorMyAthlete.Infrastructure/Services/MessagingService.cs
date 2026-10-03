using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Messaging;
using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class MessagingService(AppDbContext db, IMessageFilter messageFilter) : IMessagingService
{
    public async Task<IReadOnlyList<ThreadSummaryDto>> GetThreadsForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var threads = await db.MessageThreads
            .Where(t => t.AthleteUserId == userId || t.SponsorUserId == userId)
            .OrderByDescending(t => t.LastMessageAt)
            .ToListAsync(ct);

        var result = new List<ThreadSummaryDto>();
        foreach (var thread in threads)
        {
            var lastMessage = await db.Messages
                .Where(m => m.ThreadId == thread.Id)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync(ct);

            var label = await GetCounterpartyLabelAsync(thread, userId, ct);
            result.Add(new ThreadSummaryDto(thread.Id, label, lastMessage?.Body, thread.LastMessageAt));
        }

        return result;
    }

    public async Task<ThreadDetailDto> GetOrCreateThreadWithAthleteAsync(Guid sponsorUserId, Guid athleteProfileId, CancellationToken ct = default)
    {
        var athleteProfile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.Id == athleteProfileId, ct)
            ?? throw new KeyNotFoundException("Athlete profile not found.");

        var thread = await db.MessageThreads
            .Include(t => t.Messages)
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
            await db.SaveChangesAsync(ct);
        }

        return await BuildThreadDetailAsync(thread, sponsorUserId, ct);
    }

    public async Task<ThreadDetailDto> GetThreadAsync(Guid userId, Guid threadId, CancellationToken ct = default)
    {
        var thread = await db.MessageThreads
            .Include(t => t.Messages)
            .SingleOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new KeyNotFoundException("Thread not found.");

        EnsureParty(thread, userId);
        return await BuildThreadDetailAsync(thread, userId, ct);
    }

    public async Task<SendMessageResult> SendMessageAsync(Guid userId, Guid threadId, SendMessageRequest request, CancellationToken ct = default)
    {
        var thread = await db.MessageThreads.SingleOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new KeyNotFoundException("Thread not found.");
        EnsureParty(thread, userId);

        // External-link gating (brief 7a.1) will check thread.Deal?.Status once the concierge deal
        // flow (milestone 6) exists — no deals are wired up yet, so links are always blocked for now.
        var filterResult = messageFilter.Evaluate(request.Body, dealCompleted: false);

        if (filterResult.IsBlocked)
        {
            db.BlockedMessageAttempts.Add(new BlockedMessageAttempt
            {
                Id = Guid.NewGuid(),
                ThreadId = threadId,
                SenderUserId = userId,
                AttemptedBody = request.Body,
                MatchedReason = filterResult.Reason!,
                AttemptedAt = DateTimeOffset.UtcNow,
            });

            var user = await db.Users.SingleAsync(u => u.Id == userId, ct);
            user.BlockedAttemptCount += 1;
            if (user.BlockedAttemptCount >= 3 && !user.IsFlagged)
            {
                user.IsFlagged = true;
                db.AdminFlags.Add(new AdminFlag
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Reason = $"Reached {user.BlockedAttemptCount} blocked message attempts.",
                    CreatedAt = DateTimeOffset.UtcNow,
                });
            }

            await db.SaveChangesAsync(ct);
            return new SendMessageResult(true, filterResult.Reason, null);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ThreadId = threadId,
            SenderUserId = userId,
            Body = request.Body,
            SentAt = DateTimeOffset.UtcNow,
        };
        db.Messages.Add(message);
        thread.LastMessageAt = message.SentAt;
        await db.SaveChangesAsync(ct);

        return new SendMessageResult(false, null, new MessageDto(message.Id, userId, true, message.Body, message.SentAt));
    }

    private static void EnsureParty(MessageThread thread, Guid userId)
    {
        if (thread.AthleteUserId != userId && thread.SponsorUserId != userId)
            throw new UnauthorizedAccessException("You are not a party to this thread.");
    }

    private async Task<ThreadDetailDto> BuildThreadDetailAsync(MessageThread thread, Guid viewerUserId, CancellationToken ct)
    {
        var label = await GetCounterpartyLabelAsync(thread, viewerUserId, ct);
        var messages = thread.Messages
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto(m.Id, m.SenderUserId, m.SenderUserId == viewerUserId, m.Body, m.SentAt))
            .ToList();

        return new ThreadDetailDto(thread.Id, thread.AthleteUserId, thread.SponsorUserId, label, messages);
    }

    private async Task<string> GetCounterpartyLabelAsync(MessageThread thread, Guid viewerUserId, CancellationToken ct)
    {
        // Brief 5.4 / 7a: sponsors stay anonymous to athletes until they message, so an athlete
        // sees a generic label rather than the sponsor's identity.
        if (thread.AthleteUserId == viewerUserId)
            return "Sponsor";

        var athleteProfile = await db.AthleteProfiles.SingleOrDefaultAsync(a => a.UserId == thread.AthleteUserId, ct);
        return athleteProfile?.Sport is { Length: > 0 } sport ? $"{sport} athlete" : "Athlete";
    }
}
