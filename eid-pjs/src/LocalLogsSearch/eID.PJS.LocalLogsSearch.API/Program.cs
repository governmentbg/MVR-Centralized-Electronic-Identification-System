using Microsoft.Extensions.Hosting;
using Serilog;
using eID.PJS.LocalLogsSearch.API;
using eID.PJS.LocalLogsSearch.Service;

Console.Title = ApplicationName;
Console.WriteLine($"({ApplicationName}) is starting...");


try
{
    var hostBuilder = CreateHostBuilder(args);

    Console.WriteLine($"Building host ({ApplicationName})...");

    var host = hostBuilder.Build();

    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Information))
    {
        Log.Information("Starting host ({ApplicationName})...", ApplicationName);
    }
    else
    {
        Console.WriteLine("Starting host ({0})...", ApplicationName);
    }

    Log.Information($"Suggested MaxDegreeOfParallelism: {SystemExtensions.SuggestMaxDegreeOfParallelism()}");

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

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((context, configuration) =>
        {
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
                Log.Warning($"Trying to ensure valid processor count result in: '{result}'." );
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

/// <summary>
/// Application entry point
/// </summary>
public partial class Program
{
    private static readonly string? Namespace = typeof(Startup).Namespace;
    /// <summary>
    /// The name of the application
    /// </summary>
    public static readonly string ApplicationName = Namespace?.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1) ?? "DefaultApplicationName";
}

