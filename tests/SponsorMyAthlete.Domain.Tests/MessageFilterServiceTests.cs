using SponsorMyAthlete.Domain.Services;
using Xunit;

namespace SponsorMyAthlete.Domain.Tests;

public class MessageFilterServiceTests
{
    private readonly MessageFilterService _filter = new();

    [Theory]
    [InlineData("email me at jane@example.com", "email_address")]
    [InlineData("call me on 0412 345 678", "phone_number")]
    [InlineData("reach me on +61412345678", "phone_number")]
    [InlineData("let's chat on WhatsApp instead", "off_platform_app_reference")]
    [InlineData("just call me tonight", "off_platform_contact_phrase")]
    [InlineData("check this out: https://example.com/deal", "external_link")]
    public void BlocksDisintermediationAttempts(string body, string expectedReason)
    {
        var result = _filter.Evaluate(body, dealCompleted: false);
        Assert.True(result.IsBlocked);
        Assert.Equal(expectedReason, result.Reason);
    }

    [Fact]
    public void AllowsOrdinaryMessages()
    {
        var result = _filter.Evaluate("Thanks for the info, I'm keen to talk about a package.", dealCompleted: false);
        Assert.False(result.IsBlocked);
    }

    [Fact]
    public void AllowsExternalLinks_OnceDealIsCompleted()
    {
        var result = _filter.Evaluate("Here's the proof of the post: https://instagram.com/p/xyz", dealCompleted: true);
        Assert.False(result.IsBlocked);
    }
}
