using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Infrastructure.Persistence.Configurations;

public class AthleteProfileConfiguration : IEntityTypeConfiguration<AthleteProfile>
{
    public void Configure(EntityTypeBuilder<AthleteProfile> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.UserId).IsUnique();

        builder.Property(a => a.CompetitiveLevel).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.VerificationStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.SubscriptionStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Bio).HasMaxLength(3000);

        builder.HasIndex(a => new { a.Sport, a.State });
        builder.HasIndex(a => a.VerificationStatus);

        builder.HasMany(a => a.Accomplishments)
            .WithOne()
            .HasForeignKey(a => a.AthleteProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Photos)
            .WithOne()
            .HasForeignKey(p => p.AthleteProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.SponsorshipPackages)
            .WithOne()
            .HasForeignKey(p => p.AthleteProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
