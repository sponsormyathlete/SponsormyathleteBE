using SponsorMyAthlete.Domain;

namespace SponsorMyAthlete.Domain.Services;

/// <summary>Brief 2.2/2.3: 10% platform fee on cash deals, 5% of declared retail value on non-cash deals.</summary>
public static class DealFeeCalculator
{
    public const decimal CashFeeRate = 0.10m;
    public const decimal NonCashFeeRate = 0.05m;

    public static decimal CalculateFee(DealType type, decimal declaredValue)
    {
        if (declaredValue < 0)
            throw new ArgumentOutOfRangeException(nameof(declaredValue), "Declared value cannot be negative.");

        var rate = type == DealType.Cash ? CashFeeRate : NonCashFeeRate;
        return Math.Round(declaredValue * rate, 2, MidpointRounding.AwayFromZero);
    }
}
