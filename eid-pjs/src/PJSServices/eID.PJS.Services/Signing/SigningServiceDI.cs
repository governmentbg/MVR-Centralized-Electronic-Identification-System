using eID.PJS.Services.Signing;

using eID.PJS.AuditLogging;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using eID.PJS.Services.OpenSearch;
using eID.PJS.Services.Verification;
using eID.PJS.Services.TimeStamp;

namespace eID.PJS.Services.Archiving;

#nullable disable
public static class SigningServiceDI
{
    public static void RegisterSigningService(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentException(nameof(configuration));

        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(SigningServiceSettings));

            return configuration.GetSection(nameof(SigningServiceSettings)).Get<SigningServiceSettings>();
        });

        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(SigningSchedulerSettings));

            return configuration.GetSection(nameof(SigningSchedulerSettings)).Get<SigningSchedulerSettings>();
        });

        services.AddSingleton(svc =>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(SignServerProviderSettings));

            return configuration.GetSection(nameof(SignServerProviderSettings)).Get<SignServerProviderSettings>();
        });

        services.AddScoped<ICryptoKeyProvider, ConfigurationCryptoKeyProvider>();
        services.AddScoped<IFileChecksumAlgorhitm, HMACSignAlgorhitm>();
        services.AddScoped<IProcessingService<SigningServiceState>, SigningService>();
        services.AddScoped<ITimeStampProvider, SignServerRESTProvider>();

        services.AddHostedService<SigningScheduler>();

    }

    public static void ConfigureSigningService(this IServiceProvider services)
    {
        var status = services.GetRequiredService<GlobalStatus>();
        var svcConfig = services.GetRequiredService<SigningServiceSettings>();
        var schedulerConfig = services.GetRequiredService<SigningSchedulerSettings>();

        status.SigningServiceStatus.ServiceConfiguration = svcConfig;
        status.SigningServiceStatus.SchedulerConfiguration = schedulerConfig;
    }
}

