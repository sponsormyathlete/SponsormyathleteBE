namespace SponsorMyAthlete.Application.Messaging;

public record MessageDto(Guid Id, Guid SenderUserId, bool IsMine, string Body, DateTimeOffset SentAt);

public record ThreadSummaryDto(
    Guid Id,
    string CounterpartyLabel,
    string? LastMessagePreview,
    DateTimeOffset LastMessageAt
);

public record ThreadDetailDto(
    Guid Id,
    Guid AthleteUserId,
    Guid SponsorUserId,
    string CounterpartyLabel,
    IReadOnlyList<MessageDto> Messages
);

public record SendMessageRequest(string Body);

/// <summary>
/// Blocked messages are never persisted as real messages (brief 7a.2) — the caller checks
/// <see cref="Blocked"/> and shows the system notice instead of the message appearing in the thread.
/// </summary>
public record SendMessageResult(bool Blocked, string? BlockReason, MessageDto? Message);
