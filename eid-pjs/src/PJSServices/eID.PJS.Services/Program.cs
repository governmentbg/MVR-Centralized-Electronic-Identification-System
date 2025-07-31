using eID.PJS.Services.Verification;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace eID.PJS.Services
{
    internal class Program
    {
        public static readonly string ApplicationName = "PJSServices";
        public static Guid InstanceId { get; } = Guid.NewGuid();
        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine(":: PJS Services ::");

                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();

                Log.Information($"Suggested MaxDegreeOfParallelism: {SystemExtensions.SuggestMaxDegreeOfParallelism()}");

                MigrateDbContext(host);

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
#if VERBOSE
                    Console.WriteLine("VERBOSE: Configuring Serilog");
#endif
                    configuration.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("SystemId", ApplicationName);
                })
               .ConfigureAppConfiguration((hbc, cb) =>
               {
                   var result = SystemExtensions.EnsureValidProcessorCountInLinux(hbc);

                   if (!string.IsNullOrEmpty(result))
                   {
                       Log.Error($"Trying to ensure valid processor count result in: '{result}'.");
                   }

                   if (OperatingSystem.IsLinux())
                   {
                       Log.Information($"Total processor count accepted: {SystemExtensions.GetLinuxTotalProcessorCount()}");
                   }


               })
               .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void MigrateDbContext(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<VerificationExclusionsDbContext>();
            var logger = services.GetRequiredService<ILogger<VerificationExclusionsDbContext>>();

            AddInformation($"Migrating database associated with context {nameof(VerificationExclusionsDbContext)}");

            context.Database.Migrate();

            //context.Seed(logger);
        }

        static void AddInformation(string text)
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Information))
            {
                Log.Information(text);
            }
            else
            {
                Console.WriteLine(text);
            }
        }
    }
}
