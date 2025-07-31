using eID.PJS.Application.Consumers;
using eID.PJS.Application.Jobs;
using eID.PJS.Application.Options;
using eID.PJS.Contracts;
using eID.PJS.Service;
using eID.PJS.Service.Admin;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using OpenSearch.Net;
using Prometheus;
using Quartz;
using RabbitMQ.Client;

namespace eID.PJS.Application;

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
        services.AddOptions<StorageOptions>().BindConfiguration(nameof(StorageOptions));

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
            //.AddNpgSql(Configuration.GetConnectionString("DefaultConnection"))
            ;

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

        // Database

        // Services
        services.AddSingleton<IOpenSearchClient>((serviceProvider) =>
        {
            var openSearchOptions = Configuration.GetSection(nameof(OpenSearchOptions)).Get<OpenSearchOptions>();
            openSearchOptions.Validate();

            var nodes = openSearchOptions.NodeUrls.Select(u => new Uri(u));

            var pool = new StaticConnectionPool(nodes);
            var settings = new ConnectionSettings(pool);

            settings
                .ServerCertificateValidationCallback((obj, cert, chain, sslPolicyErrors) => { return true; })
                .BasicAuthentication(openSearchOptions.BasicAuthentiction.Username, openSearchOptions.BasicAuthentiction.Password)
                .DefaultIndex(openSearchOptions.Index)
                .MaximumRetries(openSearchOptions.MaximumRetries)
                .RequestTimeout(TimeSpan.FromSeconds(openSearchOptions.RequestTimeoutInSeconds))
                .MaxRetryTimeout(TimeSpan.FromSeconds(openSearchOptions.MaxRetryTimeoutInSeconds));

            var client = new OpenSearchClient(settings);

            return client;
        });
        services.AddScoped<OpenSearchCollectorService>();
        services.AddScoped<AdminOpenSearchCollectorService>();

        var aesOptions = new AesOptions();
        Configuration.Bind(nameof(AesOptions), aesOptions);
        aesOptions.Validate();
        EncryptionHelper.SetEncryptionKey(aesOptions.Key);

        services.Configure<ClearLogOptions>(Configuration.GetSection(nameof(ClearLogOptions)));
        services.AddQuartz(q =>
        {
            q.SchedulerId = Program.Namespace;
            q.SchedulerName = Program.Namespace;

            q.UseMicrosoftDependencyInjectionJobFactory();

            var clearLogOptions = new ClearLogOptions();
            Configuration.Bind(nameof(ClearLogOptions), clearLogOptions);
            clearLogOptions.IsValid();

            q.ScheduleJob<ClearLogFilesJob>(trigger => trigger
                .WithIdentity(nameof(ClearLogFilesJob))
                .StartAt(DateTimeOffset.UtcNow.AddMinutes(2))
                .WithCronSchedule(clearLogOptions.CronTab));

            // convert time zones using converter that can handle Windows/Linux differences
            q.UseTimeZoneConverter();
        });

        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
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
    }
}
