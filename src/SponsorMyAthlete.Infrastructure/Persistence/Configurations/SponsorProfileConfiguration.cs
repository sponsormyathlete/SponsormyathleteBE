using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Infrastructure.Persistence.Configurations;

public class SponsorProfileConfiguration : IEntityTypeConfiguration<SponsorProfile>
{
    public void Configure(EntityTypeBuilder<SponsorProfile> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId).IsUnique();

        builder.Property(s => s.AttendEvents).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.AthleteLevelPreference).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.ReachImportance).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.SponsorshipType).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.RelationshipStyle).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.AboutBlurb).HasMaxLength(3000);
        builder.Property(s => s.BudgetMin).HasPrecision(10, 2);
        builder.Property(s => s.BudgetMax).HasPrecision(10, 2);
        builder.Property(s => s.Abn).HasMaxLength(11);

        builder.HasMany(s => s.Wishlists)
            .WithOne()
            .HasForeignKey(w => w.SponsorProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
