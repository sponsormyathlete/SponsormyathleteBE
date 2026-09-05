using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorMyAthlete.Domain.Entities;

namespace SponsorMyAthlete.Infrastructure.Persistence.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.DeclaredValue).HasPrecision(10, 2);
        builder.Property(d => d.PlatformFeeAmount).HasPrecision(10, 2);
    }
}

public class MessageThreadConfiguration : IEntityTypeConfiguration<MessageThread>
{
    public void Configure(EntityTypeBuilder<MessageThread> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Deal)
            .WithMany()
            .HasForeignKey(t => t.DealId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Messages)
            .WithOne()
            .HasForeignKey(m => m.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Body).IsRequired().HasMaxLength(4000);
    }
}

public class BlockedMessageAttemptConfiguration : IEntityTypeConfiguration<BlockedMessageAttempt>
{
    public void Configure(EntityTypeBuilder<BlockedMessageAttempt> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.AttemptedBody).IsRequired().HasMaxLength(4000);
        builder.Property(b => b.MatchedReason).IsRequired().HasMaxLength(50);
    }
}

public class ProfileViewConfiguration : IEntityTypeConfiguration<ProfileView>
{
    public void Configure(EntityTypeBuilder<ProfileView> builder)
    {
        builder.HasKey(v => v.Id);
        builder.HasIndex(v => new { v.AthleteProfileId, v.ViewedAt });
    }
}

public class ProfileSaveConfiguration : IEntityTypeConfiguration<ProfileSave>
{
    public void Configure(EntityTypeBuilder<ProfileSave> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.AthleteProfileId, s.SponsorUserId }).IsUnique();
    }
}

public class AdminFlagConfiguration : IEntityTypeConfiguration<AdminFlag>
{
    public void Configure(EntityTypeBuilder<AdminFlag> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Reason).IsRequired().HasMaxLength(500);
    }
}
