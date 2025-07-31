using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using eID.PJS.Services.Archiving;
using eID.PJS.Services.Entities;
using eID.PJS.Services.LogFileMover;
using eID.PJS.Services.Verification;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace eID.PJS.Services;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
#if VERBOSE
                Console.WriteLine("VERBOSE: AddAuthentication");
#endif
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
#if VERBOSE
                Console.WriteLine("VERBOSE: AddJwtBearer");
#endif
                Configuration.Bind(nameof(JwtBearerOptions), options);
            });


        services
            .AddApiVersioning(options =>
            {
#if VERBOSE
                Console.WriteLine("VERBOSE: AddApiVersioning");
#endif
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
#if VERBOSE
                Console.WriteLine("VERBOSE: AddApiExplorer");
#endif
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#if VERBOSE
        Console.WriteLine("VERBOSE: AddSwaggerGen");
#endif
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PJS Services",
                Version = "v1",
                Description = "PJS Services - Description"
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


#if VERBOSE
        Console.WriteLine("VERBOSE: AddHealthChecks");
#endif
        // Heath check section. Add all used infrastructure
        services.AddHealthChecks();

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(2);
            options.Predicate = (check) => check.Tags.Contains("ready");
        });

        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        // Just in case set globaly to use StringEnumConverter
        JsonConvert.DefaultSettings = (() =>
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        });

        // In case of unhandled exception this service adds in response an error as ProblemDetails structure. See more: https://tools.ietf.org/html/rfc7231
        services.AddProblemDetails(opts =>
        {
            // Set up our RequestId as "traceId"
            opts.GetTraceId = (ctx) =>
                ctx.Request?.Headers?.RequestId;

            opts.IncludeExceptionDetails = (ctx, ex) =>
            {
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
        });

        services.AddCors(options =>
        {
#if VERBOSE
            Console.WriteLine("VERBOSE: AddCors");
#endif
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

#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterHttpClientWithPollyAndCert");
#endif
        // HttpClient and Certificates
        services.RegisterHttpClientWithPollyAndCert(Configuration);
#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterTimeStampServerCertificate");
#endif
        services.RegisterTimeStampServerCertificate(Configuration);

#if VERBOSE
        Console.WriteLine("VERBOSE: AddDbContextFactory");
#endif
        // Database
        services.AddDbContextFactory<VerificationExclusionsDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                );

#if VERBOSE
        Console.WriteLine("VERBOSE: AddAuthorization");
#endif
        services.AddAuthorization();

        services.AddSingleton<GlobalStatus>();

#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterCommandStateProvider");
#endif
        services.RegisterCommandStateProvider(Configuration);

#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterSigningService");
#endif
        services.RegisterSigningService(Configuration);

#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterVerificationService");
#endif
        services.RegisterVerificationService(Configuration);

#if VERBOSE
        Console.WriteLine("VERBOSE: RegisterLogFileMoverService");
#endif
        services.RegisterLogFileMover();
    }


    /// <summary>
    /// Configures the specified application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The env.</param>
    /// <param name="apiVersionDescriptionProvider">The API version description provider.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {

#if VERBOSE
        Console.WriteLine("VERBOSE: Begin Configure");
#endif
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions());

            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
            {
                Predicate = _ => false
            });
        });

#if VERBOSE
        Console.WriteLine("VERBOSE: ConfigureVerificationService");
#endif
        app.ApplicationServices.ConfigureVerificationService();

#if VERBOSE
        Console.WriteLine("VERBOSE: ConfigureSigningService");
#endif
        app.ApplicationServices.ConfigureSigningService();

#if VERBOSE
        Console.WriteLine("VERBOSE: ClearCommandState");
#endif
        app.ApplicationServices.ClearCommandState();

    }
}
