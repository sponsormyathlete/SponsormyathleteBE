using Microsoft.EntityFrameworkCore;
using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<AthleteProfile> AthleteProfiles => Set<AthleteProfile>();
    public DbSet<AthleteAccomplishment> AthleteAccomplishments => Set<AthleteAccomplishment>();
    public DbSet<AthletePhoto> AthletePhotos => Set<AthletePhoto>();
    public DbSet<SponsorshipPackage> SponsorshipPackages => Set<SponsorshipPackage>();
    public DbSet<SponsorProfile> SponsorProfiles => Set<SponsorProfile>();
    public DbSet<SponsorWishlist> SponsorWishlists => Set<SponsorWishlist>();
    public DbSet<MessageThread> MessageThreads => Set<MessageThread>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<BlockedMessageAttempt> BlockedMessageAttempts => Set<BlockedMessageAttempt>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<ProfileView> ProfileViews => Set<ProfileView>();
    public DbSet<ProfileSave> ProfileSaves => Set<ProfileSave>();
    public DbSet<AdminFlag> AdminFlags => Set<AdminFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
