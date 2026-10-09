using Amazon;
using Amazon.RDS.Util;
using Amazon.Runtime;
using Amazon.Runtime.Credentials;
using Amazon.Runtime.CredentialManagement;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SponsorMyAthlete.Application.Abstractions;
using SponsorMyAthlete.Application.Admin;
using SponsorMyAthlete.Application.AthleteProfiles;
using SponsorMyAthlete.Application.Messaging;
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
        var dataSource = BuildDataSource(configuration);
        services.AddSingleton(dataSource);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

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
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IAdminFlagService, AdminFlagService>();
        services.AddSingleton<IMessageFilter, MessageFilterService>();

        return services;
    }

    /// <summary>
    /// Local Postgres uses the password in the connection string. Aurora (express configuration)
    /// has no password — it accepts only short-lived IAM tokens, so when Database:IamAuthRegion is
    /// set the pool fetches a fresh token every 10 minutes (tokens are valid for 15).
    /// </summary>
    private static NpgsqlDataSource BuildDataSource(IConfiguration configuration)
    {
        var builder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Postgres"));

        var iamRegion = configuration["Database:IamAuthRegion"];
        if (!string.IsNullOrEmpty(iamRegion))
        {
            var region = RegionEndpoint.GetBySystemName(iamRegion);
            var credentials = ResolveAwsCredentials(configuration["Database:AwsProfile"]);

            builder.UsePeriodicPasswordProvider(
                (settings, _) => ValueTask.FromResult(
                    RDSAuthTokenGenerator.GenerateAuthToken(credentials, region, settings.Host!, settings.Port, settings.Username!)),
                successRefreshInterval: TimeSpan.FromMinutes(10),
                failureRefreshInterval: TimeSpan.FromSeconds(10));
        }

        return builder.Build();
    }

    private static AWSCredentials ResolveAwsCredentials(string? profileName)
    {
        if (string.IsNullOrEmpty(profileName))
            return DefaultAWSCredentialsIdentityResolver.GetCredentials();

        if (!new CredentialProfileStoreChain().TryGetAWSCredentials(profileName, out var credentials))
            throw new InvalidOperationException($"AWS profile '{profileName}' not found. Run: aws login --profile {profileName}");
        return credentials;
    }
}
