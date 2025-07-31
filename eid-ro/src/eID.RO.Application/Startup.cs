using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using eID.PJS.AuditLogging;
using eID.RO.Application.Consumers;
using eID.RO.Application.Options;
using eID.RO.Application.StateMachines;
using eID.RO.Contracts;
using eID.RO.Service;
using eID.RO.Service.Database;
using eID.RO.Service.EventsRegistration;
using eID.RO.Service.Extensions;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Jobs;
using eID.RO.Service.Options;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using Prometheus;
using Quartz;
using RabbitMQ.Client;

namespace eID.RO.Application;

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
        services.AddOptions<ApplicationOptions>().BindConfiguration(nameof(ApplicationOptions));
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

        services.AddQuartz(q =>
        {
            q.SchedulerName = "MassTransit-Scheduler";
            q.SchedulerId = "AUTO";

            q.UseMicrosoftDependencyInjectionJobFactory();

            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            q.UseTimeZoneConverter();

            q.UsePersistentStore(s =>
            {
                s.UseProperties = true;
                s.RetryInterval = TimeSpan.FromSeconds(15);

                s.UsePostgres(Configuration.GetConnectionString("QuartzNet"));

                s.UseJsonSerializer();

                s.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
            });

            services.Configure<ExpiringEmpowermentsNotificationJobOptions>(Configuration.GetSection(nameof(ExpiringEmpowermentsNotificationJobOptions)));
            var expiringEmpowermentsNotificationJobOptions = new ExpiringEmpowermentsNotificationJobOptions();
            Configuration.Bind(nameof(ExpiringEmpowermentsNotificationJobOptions), expiringEmpowermentsNotificationJobOptions);
            expiringEmpowermentsNotificationJobOptions.Validate();

            q.AddJob<ExpiringEmpowermentsNotificationJob>(opts => opts.WithIdentity(nameof(ExpiringEmpowermentsNotificationJob)));
            q.AddTrigger(opts => opts
                .ForJob(nameof(ExpiringEmpowermentsNotificationJob))
                .WithIdentity(nameof(ExpiringEmpowermentsNotificationJob))
                .WithCronSchedule(expiringEmpowermentsNotificationJobOptions.CronPeriod)
            );
        });
        // MessageBus
        services.AddMassTransit(mt =>
        {
            mt.AddPublishMessageScheduler();

            mt.AddQuartzConsumers();

            mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(Program.ApplicationName, false));
            // Consumers
            mt.AddConsumersFromNamespaceContaining<BaseConsumer>();
            mt.AddConsumer<NotificationsConsumer, NotificationsConsumerDefinition>(cfg =>
            {
                cfg.Options<BatchOptions>(options => options
                    .SetMessageLimit(100)
                    .SetTimeLimit(s: 10)
                    .SetTimeLimitStart(BatchTimeLimitStart.FromFirst)
                    .SetConcurrencyLimit(1)
                );
            });


            mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>(typeof(EmpowermentActivationStateMachineDefinition))
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.UsePostgres();
                    r.ExistingDbContext<SagasDbContext>();
                });
            mt.AddSagaStateMachine<SignaturesCollectionStateMachine, SignaturesCollectionState>(typeof(SignatureCollectionStateMachineDefinition))
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.UsePostgres();
                    r.ExistingDbContext<SagasDbContext>();
                });
            mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>(typeof(WithdrawalsCollectionStateMachineDefinition))
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.UsePostgres();
                    r.ExistingDbContext<SagasDbContext>();
                });
            mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>(typeof(EmpowermentVerificationStateMachineDefinition))
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.UsePostgres();
                    r.ExistingDbContext<SagasDbContext>();
                });

            mt.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.UsePublishMessageScheduler();

                cfg.UsePrometheusMetrics(serviceName: Program.ApplicationName);
                cfg.UseNewtonsoftJsonSerializer();
                cfg.UseNewtonsoftJsonDeserializer();
                cfg.UseMessageRetry(options =>
                {
                    options.Handle<NpgsqlException>();
                    options
                        .Incremental(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
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
        services.AddDbContext<SagasDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"), m =>
                    {
                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                        m.MigrationsHistoryTable($"__SagasMigrationsHistory");
                    })
                );

        //Http clients with Polly
        var applicationUrls = Configuration.GetSection(nameof(ApplicationUrls)).Get<ApplicationUrls>() ?? new ApplicationUrls();
        applicationUrls.Validate();

        services
            .AddHttpClient("PAN", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PanHostUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .UseHttpClientMetrics();

        services
            .AddHttpClient("PAN-Public", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PanPublicHostUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .UseHttpClientMetrics();

        services
            .AddHttpClient("Integrations", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.IntegrationsHostUrl);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRapidRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        services
            .AddHttpClient<IVerificationService, VerificationService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.PivrHostUrl);
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
            .AddHttpClient("Signing", httpClient =>
            {
                httpClient.BaseAddress = new Uri(applicationUrls.SigningHostUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(300);
            })
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
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
            .AddHttpMessageHandler<ObtainKeycloakTokenHandler>()
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                return ApplicationPolicyRegistry.GetRetryPolicy(logger);
            })
            .UseHttpClientMetrics();

        services.AddAuditLog(Configuration);
        // Services
        services.AddTransient<EventRegistrationService>();
        services.AddTransient<IEventsRegistrator, EventsRegistrator>();
        services.AddTransient<NotificationSenderService>();
        services.AddTransient<INotificationSender, NotificationSender>();
        services.AddTransient<IKeycloakCaller, KeycloakCaller>();
        services.AddScoped<EmpowermentsService>();
        services.AddScoped<OpenDataService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ObtainKeycloakTokenHandler>();
        services.AddScoped<IMpozeiCaller, MpozeiCaller>();
        services.AddScoped<INumberRegistrator, NumberRegistrator>();


        var aesOptions = new AesOptions();
        Configuration.Bind(nameof(AesOptions), aesOptions);
        aesOptions.Validate();
        Service.Database.EncryptionHelper.SetEncryptionKey(aesOptions.Key);
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
