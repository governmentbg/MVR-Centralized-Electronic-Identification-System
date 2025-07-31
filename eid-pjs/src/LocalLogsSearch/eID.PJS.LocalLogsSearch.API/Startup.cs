using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using eID.PJS.LocalLogsSearch.Service;
using eID.PJS.LocalLogsSearch.Service.Entities;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

#nullable disable

namespace eID.PJS.LocalLogsSearch.API
{
    /// <summary>
    /// Startup class that canfigures the services and DI
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
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
                    // TODO: DO NOT LEAVE IT IN PROD
                    options.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };
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
                    Title = "eID - Local Logs Search API",
                    Version = "v1",
                    Description = "Подсистема \"Журнал на събитията\" (ПЖС)"
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
            services.AddAuthorization();

            services.AddSingleton(svc =>
            {
                if (Configuration == null) throw new ArgumentNullException(nameof(AuditLogSearchSettings));

                return Configuration.GetSection(nameof(AuditLogSearchSettings)).Get<AuditLogSearchSettings>();
            });

            services.AddScoped<AuditLogsFileService>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="apiVersionDescriptionProvider"></param>
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

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions());

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    Predicate = _ => false
                });
            });
        }
    }
}
