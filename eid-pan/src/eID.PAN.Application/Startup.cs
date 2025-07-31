using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using eID.PAN.Application.Consumers;
using eID.PAN.Application.Options;
using eID.PAN.Contracts;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Options;
using FirebaseAdmin;
using MailKit.Net.Smtp;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Prometheus;
using RabbitMQ.Client;
using Twilio;

namespace eID.PAN.Application;

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
        services.AddOptions<SmtpOptions>().BindConfiguration(nameof(SmtpOptions));
        services.AddOptions<TwilioOptions>().BindConfiguration(nameof(TwilioOptions));
        services.AddOptions<AesOptions>().BindConfiguration(nameof(AesOptions));
        services.AddOptions<KeycloakOptions>().BindConfiguration(nameof(KeycloakOptions));
        services.AddOptions<ApplicationUrls>().BindConfiguration(nameof(ApplicationUrls));

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

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
                // Database server will be changed in the future
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"))
                );

        services
            .AddHttpClient(ApplicationPolicyRegistry.HttpClientWithRetryPolicy)
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

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
            })
            .UseHttpClientMetrics();

        services
            .AddHttpClient(MpozeiCaller.HTTPClientName, httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.MpozeiHostUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();


        // Services
        services.AddScoped<SmtpConfigurationsService>();
        services.AddScoped<NotificationsService>();
        services.AddScoped<NotificationChannelsService>();
        services.AddScoped<UserNotificationsService>();
        services.AddScoped<CommunicationsService>();
        services.AddScoped<ISmtpClient, SmtpClient>(services =>
        {
            return new SmtpClient { CheckCertificateRevocation = false };
        });
        services.AddScoped<UserNotificationChannelsService>();
        services.AddScoped<IPushNotificationSender, PushNotificationSender>();
        services.AddScoped<IMpozeiCaller, MpozeiCaller>();
        services.AddScoped<IKeycloakCaller, KeycloakCaller>();
        FirebaseApp.Create();

        var twilioOptions = new TwilioOptions();
        Configuration.Bind(nameof(TwilioOptions), twilioOptions);
        TwilioClient.Init(twilioOptions.AccountSid, twilioOptions.AuthToken);

        services.AddScoped<ISmsSender, TwilioSmsSender>();
        services.AddScoped<IHttpCallbackSender, HttpCallbackSender>();
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
    }
}
