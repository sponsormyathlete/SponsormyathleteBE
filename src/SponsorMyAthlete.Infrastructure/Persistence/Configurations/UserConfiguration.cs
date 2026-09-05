using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Auth0Sub).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Auth0Sub).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(u => u.AthleteProfile)
            .WithOne(a => a.User)
            .HasForeignKey<AthleteProfile>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.SponsorProfile)
            .WithOne(s => s.User)
            .HasForeignKey<SponsorProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
