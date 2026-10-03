using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Application.Admin;
using SponsorMyAthlete.Infrastructure.Persistence;

namespace SponsorMyAthlete.Infrastructure.Services;

public class AdminFlagService(AppDbContext db) : IAdminFlagService
{
    public async Task<IReadOnlyList<AdminFlagDto>> GetFlagsAsync(bool includeResolved, CancellationToken ct = default)
    {
        var query = db.AdminFlags.AsQueryable();
        if (!includeResolved)
            query = query.Where(f => !f.Resolved);

        var flags = await query.OrderByDescending(f => f.CreatedAt).ToListAsync(ct);
        var userIds = flags.Select(f => f.UserId).Distinct().ToList();
        var emailsByUserId = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email, ct);

        return flags
            .Select(f => new AdminFlagDto(f.Id, f.UserId, emailsByUserId.GetValueOrDefault(f.UserId, "unknown"), f.Reason, f.CreatedAt, f.Resolved))
            .ToList();
    }

    public async Task ResolveFlagAsync(Guid flagId, CancellationToken ct = default)
    {
        var flag = await db.AdminFlags.SingleAsync(f => f.Id == flagId, ct);
        flag.Resolved = true;
        flag.ResolvedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
