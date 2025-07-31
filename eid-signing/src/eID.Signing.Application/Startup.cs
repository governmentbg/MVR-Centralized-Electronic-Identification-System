using eID.Authorization.Keycloak;
using eID.Signing.Application.Consumers;
using eID.Signing.Application.Jobs;
using eID.Signing.Application.Options;
using eID.Signing.Contracts;
using eID.Signing.Service;
using eID.Signing.Service.Database;
using eID.Signing.Service.Options;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Prometheus;
using Quartz;
using RabbitMQ.Client;

namespace eID.Signing.Application;

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
        services.AddOptions<ApplicationUrls>().BindConfiguration(nameof(ApplicationUrls));
        services.AddOptions<AutomaticRemoteSigningOptions>().BindConfiguration(nameof(AutomaticRemoteSigningOptions));
        services.AddOptions<KeycloakOptions>().BindConfiguration(nameof(KeycloakOptions));
        services.AddOptions<TokenExpirationNotificationOptions>().BindConfiguration(nameof(TokenExpirationNotificationOptions));

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
            .AddNpgSql(Configuration.GetConnectionString("DefaultConnection"));

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
                // Database server will be changed in the future
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"))
                );

        // Cache
        services.AddStackExchangeRedisCache(options =>
        {
            var redisSettings = new RedisOptions();
            Configuration.Bind(nameof(RedisOptions), redisSettings);

            options.Configuration = redisSettings.ConnectionString;
        });

        // MessageBus
        services.AddMassTransit(mt =>
        {
            mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(Program.ApplicationName, false));
            // Consumers
            mt.AddConsumersFromNamespaceContaining<BaseConsumer>();
            mt.UsingRabbitMq((ctx, cfg) =>
            {
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

        // HttpClients
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#alternatives-to-ihttpclientfactory
        services.AddSingleton((services) =>
        {
            var socketTimeoutInMsConfig = Configuration.GetValue<int>("SocketsHttpHandlerConnectTimeoutInMs");
            var socketTimeoutInMs = 200;
            if (socketTimeoutInMsConfig > 0)
            {
                socketTimeoutInMs = socketTimeoutInMsConfig;
            }

            return new SocketsHttpHandler { ConnectTimeout = TimeSpan.FromMilliseconds(socketTimeoutInMs) };
        });
        services
            .AddHttpClient(ApplicationConstants.HttpClientWithMetricsName).UseHttpClientMetrics();

        var applicationUrls = Configuration.GetSection(nameof(ApplicationUrls)).Get<ApplicationUrls>() ?? new ApplicationUrls();
        applicationUrls.Validate();

        services
          .AddHttpClient(KeycloakCaller.HTTPClientName, httpClient =>
          {
              httpClient.BaseAddress = new Uri(applicationUrls.KeycloakHostUrl);
          })
          .AddPolicyHandler((serviceProvider, request) =>
          {
              var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
              return ApplicationPolicyRegistry.GetRetryPolicy(logger);
          }).
          UseHttpClientMetrics();

        services
            .AddHttpClient("PAN", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PanHostUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetPANCallPolicy(logger);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .UseHttpClientMetrics();


        services.AddQuartz(q =>
        {
            q.SchedulerId = Program.Namespace;
            q.SchedulerName = Program.Namespace;

            q.UseMicrosoftDependencyInjectionJobFactory();

            q.ScheduleJob<DailyCheckBoricaTokenExpirationDateJob>(trigger => trigger
                .WithIdentity(nameof(DailyCheckBoricaTokenExpirationDateJob))
                .StartNow()
                .WithCronSchedule("0 5 0 * * ?", x =>
                {
                    x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia"));
                }) // Everyday at 00:05 Bulgarian time as I suspect token expiration date is returned in Bulgarian time
            );

            // convert time zones using converter that can handle Windows/Linux differences
            q.UseTimeZoneConverter();
        });

        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        // Services
        services.AddScoped<EvrotrustSigningService>();
        services.AddScoped<BoricaSigningService>();
        services.AddScoped<KEPSigningService>();
        services.AddScoped<IKeycloakCaller, KeycloakCaller>();
        services.AddScoped<ObtainKeycloakTokenHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

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
