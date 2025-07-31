using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using eID.PAN.Contracts;
using eID.PJS.AuditLogging;
using Hellang.Middleware.ProblemDetails;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Prometheus;
using RabbitMQ.Client;

namespace eID.PAN.API.Public;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                Configuration.Bind(nameof(JwtBearerOptions), options);
            });

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("api-version"));
            })
            .AddApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "eID - PAN Public HTTP API",
                Version = "v1",
                Description = "Подсистема за автоматични нотификации (ПАН) Публична"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        services.AddSwaggerGenNewtonsoftSupport();

        services.AddOptions<RabbitMqTransportOptions>().BindConfiguration(nameof(RabbitMqTransportOptions));

        // Heath check section. Add all used infrastructure
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
            });

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(2);
            options.Predicate = (check) => check.Tags.Contains("ready");
        });

        services.AddMassTransit(mt =>
        {
            mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(Program.ApplicationName, false));

            mt.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.UsePrometheusMetrics(serviceName: Program.ApplicationName);
                cfg.ConfigureEndpoints(ctx);
                cfg.UseNewtonsoftJsonSerializer();
                cfg.UseNewtonsoftJsonDeserializer();
            });
        });

        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        // In case of unhandled exception this service adds in response an error as ProblemDetails structure. See more: https://tools.ietf.org/html/rfc7231
        services.AddProblemDetails(opts =>
        {
            // Set up our RequestId as "traceId"
            opts.GetTraceId = (ctx) =>
                ctx.Request?.Headers?.RequestId;

            opts.OnBeforeWriteDetails = (ctx, details) =>
            {
                if (ctx.Items.ContainsKey(nameof(LogEventCode))
                    && ctx.Items.ContainsKey("Payload"))
                {
                    var _auditLogger = ctx.RequestServices.GetRequiredService<AuditLogger>();
                    var logEventCode = (LogEventCode)ctx.Items[nameof(LogEventCode)];
                    var eventPayload = ctx.Items["Payload"] as SortedDictionary<string, object>;
                    eventPayload.Add("ResponseStatusCode", ctx.Response.StatusCode);
                    eventPayload.Add("Reason", "Unhandled exception");
                    _auditLogger.LogEvent(new AuditLogEvent
                    {
                        CorrelationId = ctx.Request?.Headers?.RequestId.ToString(),
                        RequesterSystemId = ctx.User.Claims.FirstOrDefault(d => string.Equals(d.Type, "SystemId", StringComparison.InvariantCultureIgnoreCase))?.Value,
                        RequesterSystemName = ctx.User.Claims.FirstOrDefault(d => string.Equals(d.Type, "SystemName", StringComparison.InvariantCultureIgnoreCase))?.Value,
                        RequesterUserId = ctx.User.Claims.FirstOrDefault(d => d.Type == Claims.CitizenProfileId)?.Value ?? "Unknown user",
                        TargetUserId = ctx.Items["TargetUserId"]?.ToString(),
                        EventType = $"{logEventCode}_{LogEventLifecycle.FAIL}",
                        Message = LogEventMessages.GetLogEventMessage(logEventCode, LogEventLifecycle.FAIL),
                        EventPayload = eventPayload
                    });
                }
            };

            opts.IncludeExceptionDetails = (ctx, ex) =>
            {
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
        });

        services.AddCors(options =>
        {
            var allowedOrigins = Configuration.GetSection("AllowedOrigins").Value;

            if (!string.IsNullOrWhiteSpace(allowedOrigins))
            {
                var allAllowedOrigins = allowedOrigins.Split(';')
                    .Select(x => x.Trim()).ToArray();

                options.AddPolicy("CrossOriginPolicy", builder =>
                {
                    builder.WithOrigins(allAllowedOrigins)
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                });
            }
        });

        services.AddAuditLog(Configuration);

        // Certificate authorization
        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        app.UseProblemDetails();

        if (env.IsDevelopment())
        {
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        app.UseRouting();

        app.UseCors("CrossOriginPolicy");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpMetrics(options =>
        {
            // This will preserve only the first digit of the status code.
            // For example: 200, 201, 203 -> 2xx
            options.ReduceStatusCodeCardinality();
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();

            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions());

            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
            {
                Predicate = _ => false
            });
        });
    }
}
