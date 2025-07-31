using eID.PIVR.Application.Consumers;
using eID.PIVR.Application.Jobs;
using eID.PIVR.Application.Options;
using eID.PIVR.Contracts;
using eID.PIVR.Service;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Options;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Prometheus;
using Quartz;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace eID.PIVR.Application;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Options
        services.AddOptions<RabbitMqTransportOptions>().BindConfiguration(nameof(RabbitMqTransportOptions));
        services.AddOptions<RedisOptions>().BindConfiguration(nameof(RedisOptions));
        services.AddOptions<RegiXOptions>().BindConfiguration(nameof(RegiXOptions));
        services.AddOptions<ExternalRegistersCacheOptions>().BindConfiguration(nameof(ExternalRegistersCacheOptions));

        // HealthCheck
        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(2);
            options.Predicate = (check) => check.Tags.Contains("ready");
        });
        services
            .AddHealthChecks()
            .AddRabbitMQ((serviceProvider) =>
            {
                var factory = new ConnectionFactory();
                var rabbitTransportOptions = serviceProvider.GetRequiredService<IOptions<RabbitMqTransportOptions>>();
                var rabbitOptions = rabbitTransportOptions.Value;
                factory.UserName = rabbitOptions.User;
                factory.Password = rabbitOptions.Pass;
                factory.HostName = rabbitOptions.Host;
                if (rabbitOptions.UseSsl)
                {
                    // TODO
                    factory.Ssl = new SslOption { };
                }
                return factory.CreateConnection();
            })
            .AddRedis((serviceProvider) =>
            {
                var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
                return redisOptions.Value.ConnectionString;
            })
            .AddNpgSql(Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

        // Cache
        services.AddStackExchangeRedisCache(options =>
        {
            var redisSettings = new RedisOptions();
            Configuration.Bind(nameof(RedisOptions), redisSettings);

            options.Configuration = redisSettings.ConnectionString;
        });

        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
            var configuration = ConfigurationOptions.Parse(redisOptions.Value.ConnectionString, true);
            return ConnectionMultiplexer.Connect(configuration);
        });

        var regiXOptions = Configuration.GetRequiredSection(nameof(RegiXOptions)).Get<RegiXOptions>() ?? new RegiXOptions();

        // MessageBus
        services.AddMassTransit(mt =>
        {
            mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(Program.ApplicationName, false));
            // Consumers
            mt.AddConsumersFromNamespaceContaining<BaseConsumer>();
            mt.AddConsumer<RegiXConsumer>(conf =>
            {
                conf.ConcurrentMessageLimit = regiXOptions.ConcurrentMessageLimit;
            });
            mt.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.UsePrometheusMetrics(serviceName: Program.ApplicationName);
                cfg.UseNewtonsoftJsonSerializer();
                cfg.UseNewtonsoftJsonDeserializer();
                cfg.ConfigureEndpoints(ctx);
            });
        });

        // IPublicBus is responsible for publishing events for external systems
        services.Configure<RabbitMqTransportOptions>(Configuration.GetSection(nameof(RabbitMqTransportOptions)));
        var rabbitSettings = new RabbitMqTransportOptions();
        Configuration.Bind(nameof(RabbitMqTransportOptions), rabbitSettings);
        services.AddMassTransit<IPublicBus>(mt =>
        {
            mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(Program.ApplicationName, false));

            mt.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VHost, settings =>
                {
                    settings.Username(rabbitSettings.User);
                    settings.Password(rabbitSettings.Pass);
                    if (rabbitSettings.UseSsl)
                    {
                        settings.UseSsl(sslConf =>
                        {
                            // TODO
                        });
                    }
                });
                cfg.UseNewtonsoftJsonSerializer();
            });
        });

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
                // Database server will be changed in the future
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"))
                );

        // HttpClients
        services
            .AddHttpClient<IRegiXCaller, RegiXCaller>((httpClient) =>
            {
                httpClient.BaseAddress = new Uri(regiXOptions.ServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#alternatives-to-ihttpclientfactory
                return new SocketsHttpHandler
                {
                    ConnectTimeout = TimeSpan.FromMilliseconds(regiXOptions.SocketsHttpHandlerConnectTimeoutInMs),
                    SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
                    {
                        // Skipping Certificate validation on the cloud
                        RemoteCertificateValidationCallback = delegate { return true; }
                    }
                };
            });

        services
            .AddHttpClient("Integrations", httpClient =>
            {
                var applicationUrls = Configuration.GetSection(nameof(ApplicationUrls)).Get<ApplicationUrls>() ?? new ApplicationUrls();
                applicationUrls.Validate();
                httpClient.BaseAddress = new Uri(applicationUrls.IntegrationsHostUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRapidRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        services
            .AddHttpClient("Ocsp", httpClient =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("eId-Bulgaria/1.0 (Bulgarian electronic documents)");
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        // Services
        services
            .AddHttpClient<INAIFService, NAIFService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#alternatives-to-ihttpclientfactory
                return new SocketsHttpHandler
                {
                    ConnectTimeout = TimeSpan.FromMilliseconds(regiXOptions.SocketsHttpHandlerConnectTimeoutInMs),
                    SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
                    {
                        // Skipping Certificate validation on the cloud
                        RemoteCertificateValidationCallback = delegate { return true; }
                    }
                };
            })
            .UseHttpClientMetrics();
        if (regiXOptions.UseProductionService)
        {
            services.AddScoped<IRegiXService, RegiXService>();
        }
        else
        {
            services.AddScoped<IRegiXService, RegiXFakeService>();
        }

        services.AddQuartz(q =>
        {
            q.SchedulerId = Program.Namespace;
            q.SchedulerName = Program.Namespace;

            q.UseMicrosoftDependencyInjectionJobFactory();

            q.ScheduleJob<PersistDailyApiUsageStatisticsJob>(trigger => trigger
                .WithIdentity(nameof(PersistDailyApiUsageStatisticsJob))
                .StartNow()
                .WithCronSchedule("0 5 0 * * ?") // Everyday at 00:05 UTC
            );

            // convert time zones using converter that can handle Windows/Linux differences
            q.UseTimeZoneConverter();
        });

        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        services.AddScoped<DateOfDeathService>();
        services.AddScoped<DateOfProhibitionService>();
        services.AddScoped<VerificationService>();
        services.AddScoped<ISignatureProvidersCaller, SignatureProvidersCaller>();
        services.AddScoped<IdentityChecksService>();
        services.AddScoped<IApiUsagePersistenceService, ApiUsagePersistenceService>();
        services.AddScoped<OpenDataService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseHttpMetrics(options =>
        {
            // This will preserve only the first digit of the status code.
            // For example: 200, 201, 203 -> 2xx
            options.ReduceStatusCodeCardinality();
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions());

            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
            {
                Predicate = _ => false
            });
        });
    }
}
