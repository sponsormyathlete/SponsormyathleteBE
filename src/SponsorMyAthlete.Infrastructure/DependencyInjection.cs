using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.Payments;
using SponsorMyAthlete.Application.SponsorProfiles;
using SponsorMyAthlete.Application.Users;
using SponsorMyAthlete.Domain.Services;
using SponsorMyAthlete.Infrastructure.Config;
using SponsorMyAthlete.Infrastructure.Persistence;
using SponsorMyAthlete.Infrastructure.Services;
using Stripe;

namespace SponsorMyAthlete.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));
        services.Configure<AbrOptions>(configuration.GetSection(AbrOptions.SectionName));

        var stripeSecretKey = configuration.GetSection(StripeOptions.SectionName)["SecretKey"];
        if (!string.IsNullOrEmpty(stripeSecretKey))
            StripeConfiguration.ApiKey = stripeSecretKey;

        services.AddHttpClient<IAbrClient, AbrClient>();

        services.AddScoped<IUserSyncService, UserSyncService>();
        services.AddScoped<IAthleteProfileService, AthleteProfileService>();
        services.AddScoped<ISponsorProfileService, SponsorProfileService>();
        services.AddScoped<IPaymentSetupService, PaymentSetupService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddSingleton<IMessageFilter, MessageFilterService>();

        return services;
    }
}
