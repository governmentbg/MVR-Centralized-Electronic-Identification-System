using System.Security.Cryptography.X509Certificates;
using eID.Authorization.Keycloak;
using eID.PDEAU.Application.Consumers;
using eID.PDEAU.Application.Jobs;
using eID.PDEAU.Application.Options;
using eID.PDEAU.Contracts;
using eID.PDEAU.Service;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Events;
using eID.PDEAU.Service.Options;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Prometheus;
using Quartz;
using RabbitMQ.Client;

namespace eID.PDEAU.Application;

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
        services.AddOptions<KeycloakOptions>().BindConfiguration(nameof(KeycloakOptions));
        services.AddOptions<ApplicationUrls>().BindConfiguration(nameof(ApplicationUrls));
        services.AddOptions<TimestampServerOptions>().BindConfiguration(nameof(TimestampServerOptions));
        services.AddOptions<NotificationEmailsOptions>().BindConfiguration(nameof(NotificationEmailsOptions));

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
                cfg.UsePrometheusMetrics(serviceName: Program.ApplicationName);
                cfg.UseNewtonsoftJsonSerializer();                
                cfg.UseNewtonsoftJsonDeserializer();
                cfg.ConfigureNewtonsoftJsonSerializer(settings =>
                {
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    return settings;
                });
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

        //Http clients with Polly
        var applicationUrls = Configuration.GetSection(nameof(ApplicationUrls)).Get<ApplicationUrls>() ?? new ApplicationUrls();
        applicationUrls.Validate();

        services
            .AddHttpClient("PIVR", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PivrHostUrl);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .UseHttpClientMetrics();

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
            .AddHttpClient(MpozeiCaller.HTTPClientName, httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.MpozeiHostUrl);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        var timestampServerOptions = new TimestampServerOptions();
        Configuration.Bind(nameof(TimestampServerOptions), timestampServerOptions);
        timestampServerOptions.Validate();
        services
            .AddHttpClient(TimestampServerOptions.HTTP_CLIENT_NAME, httpClient =>
            {
                httpClient.BaseAddress = new Uri(timestampServerOptions.BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.ContentType, "application/timestamp-query");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.ContentType, System.Net.Mime.MediaTypeNames.Application.Json);
            })
            .ConfigurePrimaryHttpMessageHandler((s) =>
            {
                var bytes = File.ReadAllBytes(Path.GetFullPath(timestampServerOptions.CertificatePath));
                var certificate = new X509Certificate2(bytes, timestampServerOptions.CertificatePass);

                return new HttpClientHandler
                {
                    ClientCertificates = { certificate },
                };
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            }).
            UseHttpClientMetrics();

        services.Configure<ScrapeIISDAOptions>(Configuration.GetSection(nameof(ScrapeIISDAOptions)));
        services.AddQuartz(q =>
        {
            q.SchedulerId = Program.Namespace;
            q.SchedulerName = Program.Namespace;

            q.UseMicrosoftDependencyInjectionJobFactory();

            var scrapeOptions = new ScrapeIISDAOptions();
            Configuration.Bind(nameof(ScrapeIISDAOptions), scrapeOptions);
            scrapeOptions.IsValid();

            q.ScheduleJob<ScrapeIISDAJob>(trigger => trigger
                .WithIdentity(nameof(ScrapeIISDAJob))
                .StartAt(DateTimeOffset.UtcNow.AddMinutes(2))
                .WithCronSchedule(scrapeOptions.CronTab));

            // convert time zones using converter that can handle Windows/Linux differences
            q.UseTimeZoneConverter();
        });

        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        // Services
        services.AddScoped<IProvidersService, ProvidersService>();
        services.AddScoped<ObtainKeycloakTokenHandler>();
        services.AddScoped<IKeycloakCaller, KeycloakCaller>();
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<INotificationSender, NotificationSender>();
        services.AddTransient<EventRegistrationService>();
        services.AddTransient<IEventsRegistrator, EventsRegistrator>();
        services.AddScoped<IMpozeiCaller, MpozeiCaller>();
        services.AddScoped<IISDAScrapeService>();
        services.AddScoped<ProvidersDetailsService>();
        services.AddScoped<ITimestampService, TimestampService>();
        services.AddScoped<INumberRegistrator, NumberRegistrator>();
        services.AddScoped<OpenDataService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
        }

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

        //Fire аnd forget
        Task.Run(async () =>
        {
            var еventRegistrationService = app.ApplicationServices.GetRequiredService<EventRegistrationService>();
            await еventRegistrationService.RegisterEventsAsync();
        });
    }
}
