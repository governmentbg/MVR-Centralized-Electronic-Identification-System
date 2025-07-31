using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Extensions;
using Serilog.Formatting.Compact;

using eID.PJS.AuditLogging;
using Steeltoe.Extensions.Configuration.RandomValue;
using Steeltoe.Extensions.Configuration.Placeholder;
using Steeltoe.Extensions.Configuration;

#nullable disable

namespace AuditLogSourceApp
{
    internal class Program
    {
        public static readonly string ApplicationName = "AuditLogSourceApp";
        public static IConfiguration _config;
        private static SystemIdProvider _systemIdProvider = new SystemIdProvider();

        static int Main(string[] args)
        {

            try
            {
                Environment.SetEnvironmentVariable("SESSION_ID", Guid.NewGuid().ToString("N"));
                string systemId = null;

                if (args.Length == 1)
                {
                    systemId = args[0];
                }
                else
                {
                    systemId = _systemIdProvider.SystemId;
                }

                Environment.SetEnvironmentVariable("EID_SYSTEM_ID", systemId);

                _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddPlaceholderResolver()
                    .AddRandomValueSource()
                    .AddAuditLogConfiguration(systemId, $"module-{(new Random()).Next(0, 9)}")
                    .Build();

                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();

                Console.WriteLine($":: Audit Logging Source Generator Service SystemID: {systemId}::");

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Fatal))
                {
                    Log.Fatal(ex, "Program ({ApplicationName}) terminated unexpectedly!", ApplicationName);
                }
                else
                {
                    Console.WriteLine("Program ({0}) terminated unexpectedly! Exception: {1}", ApplicationName, ex);
                }
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName();
                })
                .EnableSerilogSelfLog()
                .ConfigureServices((svc) =>
                {
                    svc.AddScoped<ICryptoKeyProvider, DummyCryptoKeyProvider>();
                    //svc.AddScoped<ICryptoKeyProvider, ConfigurationCryptoKeyProvider>();
                    svc.AddSingleton(_config);
                    svc.AddAuditLog(_config);
                    svc.AddHostedService<AuditLogGeneratorService>();

                    //<USE IN YOUR APP> ---------------------------------------------------------------------------------------
                    svc.AddAuditLog(_config); // REQUIRED: Register the audit log service
                    //</USE IN YOUR APP> --------------------------------------------------------------------------------------

                });



    }
}
