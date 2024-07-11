using eID.PUN.Application.Consumers;
using eID.PUN.Application.Options;
using eID.PUN.Contracts;
using eID.PUN.Service;
using eID.PUN.Service.Database;
using eID.PUN.Service.Extensions;
using eID.PUN.Service.Options;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Prometheus;
using RabbitMQ.Client;

namespace eID.PUN.Application;

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
        services.AddOptions<KeycloakOptions>().BindConfiguration(nameof(KeycloakOptions));

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

        //Http PAN client for events registration
        var applicationUrls = Configuration.GetSection(nameof(ApplicationUrls)).Get<ApplicationUrls>();
        applicationUrls.Validate();

        services
            .AddHttpClient("PAN", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PanHostUrl);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        services
           .AddHttpClient(KeycloakCaller.HTTPClientName, httpClient =>
           {
               httpClient.BaseAddress = new Uri(applicationUrls.KeycloakHostUrl);
           })
            // TODO: Must be removed at some point
            .ConfigurePrimaryHttpMessageHandler((s) =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
            })
           .AddPolicyHandler((serviceProvider, request) =>
           {
               var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
               return ApplicationPolicyRegistry.GetRetryPolicy(logger);
           }).
           UseHttpClientMetrics();

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
                // Database server will be changed in the future
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"))
                );

        // Services
        services.AddScoped<CarriersService>();
        services.AddTransient<EventRegistrationService>();
        services.AddTransient<IEventsRegistrator, EventsRegistrator>();
        services.AddTransient<NotificationsService>();
        services.AddTransient<INotificationsSender, NotificationsSender>();
        services.AddScoped<IKeycloakCaller, KeycloakCaller>();
        services.AddScoped<ObtainKeycloakTokenHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
        }

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

        //Register PUN events in PAN
        Task.Run(async () =>
        {
            var еventRegistrationService = app.ApplicationServices.GetRequiredService<EventRegistrationService>();
            await еventRegistrationService.RegisterEventsAsync();
        });
    }
}
