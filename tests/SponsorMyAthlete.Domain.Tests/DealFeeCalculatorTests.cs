using SponsorMyAthlete.Domain;
using SponsorMyAthlete.Domain.Services;
using Xunit;

namespace SponsorMyAthlete.Domain.Tests;

public class DealFeeCalculatorTests
{
    [Theory]
    [InlineData(1000, 100)]
    [InlineData(500, 50)]
    [InlineData(0, 0)]
    public void CashDeals_ChargeTenPercent(decimal declaredValue, decimal expectedFee)
    {
        Assert.Equal(expectedFee, DealFeeCalculator.CalculateFee(DealType.Cash, declaredValue));
    }

    [Theory]
    [InlineData(1000, 50)]
    [InlineData(500, 25)]
    public void NonCashDeals_ChargeFivePercent(decimal declaredValue, decimal expectedFee)
    {
        Assert.Equal(expectedFee, DealFeeCalculator.CalculateFee(DealType.NonCash, declaredValue));
    }

    [Fact]
    public void NegativeDeclaredValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DealFeeCalculator.CalculateFee(DealType.Cash, -1));
    }
}
