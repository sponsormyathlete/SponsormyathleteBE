using SponsorMyAthlete.Domain.Entities;
using SponsorMyAthlete.Domain.Services;
using Xunit;

namespace SponsorMyAthlete.Domain.Tests;

public class ProfileStrengthCalculatorTests
{
    [Fact]
    public void EmptyProfile_ScoresZero()
    {
        var profile = new AthleteProfile { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var result = ProfileStrengthCalculator.Calculate(profile);
        Assert.Equal(0, result.Score);
        Assert.All(result.Items, i => Assert.False(i.Complete));
    }

    [Fact]
    public void CompleteProfile_ScoresOneHundred()
    {
        var profile = new AthleteProfile
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Organisation = "Elite Boxing Club",
            Bio = string.Join(' ', Enumerable.Repeat("word", 120)),
            InstagramHandle = "athlete_handle",
            UpdatedAt = DateTimeOffset.UtcNow,
            Accomplishments =
            [
                new AthleteAccomplishment { Description = "State champion" },
                new AthleteAccomplishment { Description = "Regional title" },
                new AthleteAccomplishment { Description = "Rookie of the year" },
            ],
            Photos =
            [
                new AthletePhoto { Url = "profile.jpg", IsProfilePhoto = true, UploadedAt = DateTimeOffset.UtcNow },
                new AthletePhoto { Url = "comp1.jpg", UploadedAt = DateTimeOffset.UtcNow },
                new AthletePhoto { Url = "comp2.jpg", UploadedAt = DateTimeOffset.UtcNow },
                new AthletePhoto { Url = "comp3.jpg", UploadedAt = DateTimeOffset.UtcNow },
            ],
            SponsorshipPackages = [new SponsorshipPackage { Title = "Gi sponsorship", Description = "Logo on gi" }],
        };

        var result = ProfileStrengthCalculator.Calculate(profile);
        Assert.Equal(100, result.Score);
    }
}
