
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
using Microsoft.AspNetCore.Hosting;

#nullable disable

namespace eID.PJS.AuditLogging.SampleApp
{
    internal class Program
    {
        public static IConfiguration _config;
        private static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string ApplicationName = Namespace?.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1) ?? "SampleApp";
        static int Main(string[] args)
        {

            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)

                    //<USE IN YOUR APP> ---------------------------------------------------------------------------------------
                    // REQUIRED: Add the audit log configuration by specifying what you'll use for the SystemId.
                    // The audit logging library will always try to use a cleaned/correct value for System ID.
                    // For example if you provide PAN.API.Public here the audit log lib will translate it to PAN-API-Public replacing all non-alphanumeric characters with dash.
                    // You can override the provided value here with the configuration of the audit log service using "systemId" property or using the environment variable EID_SYSTEM_ID.
                    .AddAuditLogConfiguration(ApplicationName, "module-1")
                    //</USE IN YOUR APP> --------------------------------------------------------------------------------------

                    .Build();

                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();

                Console.WriteLine(":: Sample Application for Audit Logging ::");

                //<USE IN YOUR APP> ---------------------------------------------------------------------------------------
                // OPTIONAL: You can log if the passed value to the AddAuditLogConfiguration is invalid and a cleaned value is used instead.
                AuditLogDIExtensions.LogIfInvalidSystemIdIsProvided();
                //</USE IN YOUR APP> --------------------------------------------------------------------------------------

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
                //.EnableSerilogSelfLog() // This is for debugging purposes
                .ConfigureServices((svc) =>
                {
                    svc.AddSingleton(_config);

                    //<USE IN YOUR APP> ---------------------------------------------------------------------------------------
                    svc.AddAuditLog(_config); // REQUIRED: Register the audit log service
                    //</USE IN YOUR APP> --------------------------------------------------------------------------------------

                    svc.AddHostedService<SampleService>();
                    
                });



    }

}
