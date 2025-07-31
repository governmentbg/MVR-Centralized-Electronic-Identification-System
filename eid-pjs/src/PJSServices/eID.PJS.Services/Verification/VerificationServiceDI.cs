using System.Configuration;
using eID.PJS.Services.AsyncExclusiveCommand;
using eID.PJS.Services.Entities;
using eID.PJS.Services.OpenSearch;

namespace eID.PJS.Services.Verification;

#nullable disable
public static class VerificationServiceDI
{
    public static void RegisterVerificationService(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(VerificationServiceSettings));

            return configuration.GetSection(nameof(VerificationServiceSettings)).Get<VerificationServiceSettings>();
        });

        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(OpenSearchManagerSettings));

            return configuration.GetSection(nameof(OpenSearchManagerSettings)).Get<OpenSearchManagerSettings>();
        });


        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(VerificationSchedulerSettings));

            return configuration.GetSection(nameof(VerificationSchedulerSettings)).Get<VerificationSchedulerSettings>();
        });


        services.AddScoped<IVerificationExclusionProvider, PostgresExclusionProvider>();
        services.AddScoped<IProcessingService<VerificationServiceStateShort>, VerificationService>();
        services.AddScoped<OpenSearchManager>();
        services.AddScoped<VerificationService>();

        services.AddHostedService<VerificationScheduler>();
    }

    public static void ConfigureVerificationService(this IServiceProvider services)
    {
        var status = services.GetRequiredService<GlobalStatus>();
        var svcConfig = services.GetRequiredService<VerificationServiceSettings>();
        var schedulerConfig = services.GetRequiredService<VerificationSchedulerSettings>();


        status.VerificationServiceStatus.ServiceConfiguration = svcConfig;
        status.VerificationServiceStatus.SchedulerConfiguration = schedulerConfig;
    }

    public static void RegisterCommandStateProvider(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var settings = configuration.GetSection(nameof(VerificationServiceSettings)).Get<VerificationServiceSettings>();

        var providerType = Type.GetType(settings.CommandStateProviderType);

        if (providerType == null)
            throw new ConfigurationErrorsException($"Invalid CommandStateProviderType provided: '{settings.CommandStateProviderType}'");



        if (providerType.FullName == typeof(InMemoryCommandStateProvider<VerificationServiceSettings>).FullName)
        {
            var memCfg = configuration.GetSection(nameof(InMemoryCommandStateProviderSettings)).Get<InMemoryCommandStateProviderSettings>();

            if (memCfg == null)
                throw new ConfigurationErrorsException("InMemoryCommandStateProviderSettings is required");

            var svc = new InMemoryCommandStateProvider<VerificationServiceStateShort>(memCfg);

            services.AddSingleton(typeof(ICommandStateProvider<VerificationServiceStateShort>), (o) => svc);

            return;
        }

        if (providerType.FullName == typeof(RedisCommandStateProvider<VerificationServiceSettings>).FullName)
        {
            var redisCfg = configuration.GetSection(nameof(RedisCommandStateProviderSettings)).Get<RedisCommandStateProviderSettings>();

            if (redisCfg == null)
                throw new ConfigurationErrorsException("RedisCommandStateProviderSettings is required");

            if (string.IsNullOrEmpty(redisCfg.ConnectionString))
                throw new ConfigurationErrorsException("RedisCommandStateProviderSettings.ConnectionString is required");

            var svc = new RedisCommandStateProvider<VerificationServiceStateShort>(redisCfg);
            services.AddSingleton(typeof(ICommandStateProvider<VerificationServiceStateShort>), (o) => svc);

            return;
        }

        throw new ConfigurationErrorsException($"Unknown ICommandStateProvider '{providerType}'");


    }

    /// <summary>Clears the state of the commands left in case the program was terminated by error or intentionally.</summary>
    /// <param name="services">The services.</param>
    public static void ClearCommandState(this IServiceProvider services)
    {
        var commandState = services.GetRequiredService<ICommandStateProvider<VerificationServiceStateShort>>();

        commandState.RemoveCommandInProgress(typeof(VerifyAllCommand).FullName ?? typeof(VerifyAllCommand).Name);
        commandState.RemoveCommandInProgress(typeof(VerifyPeriodCommand).FullName ?? typeof(VerifyPeriodCommand).Name);
    }
}

