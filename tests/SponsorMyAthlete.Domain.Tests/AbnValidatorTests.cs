using SponsorMyAthlete.Domain.Services;
using Xunit;

namespace SponsorMyAthlete.Domain.Tests;

public class AbnValidatorTests
{
    [Theory]
    [InlineData("51824753556")]
    [InlineData("51 824 753 556")]
    public void ValidAbn_PassesChecksum(string abn)
    {
        Assert.True(AbnValidator.IsValidFormat(abn));
    }

    [Theory]
    [InlineData("51824753557")]
    [InlineData("12345678901")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void InvalidAbn_FailsChecksum(string? abn)
    {
        Assert.False(AbnValidator.IsValidFormat(abn));
    }
}
