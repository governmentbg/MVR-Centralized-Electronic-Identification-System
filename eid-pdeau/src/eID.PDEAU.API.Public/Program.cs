using eID.PDEAU.API.Public;
using eID.PJS.AuditLogging;
using Serilog;

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
                .Enrich.WithProperty("SystemId", SystemName)
                .Enrich.WithProperty("ModuleId", ApplicationName);
        })
        .ConfigureAppConfiguration((hbc, cb) =>
        {
            cb.AddAuditLogConfiguration(SystemName, ApplicationName);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            //webBuilder.ConfigureKestrel((context, options) =>
            //{
            //    var urls = context.Configuration.GetValue("ASPNETCORE_URLS", "http://localhost:80");
            //    var urlList = urls.Split(';', StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var u in urlList)
            //    {
            //        if (Uri.TryCreate(u, new UriCreationOptions { }, out var uri))
            //        {
            //            options.Listen(IPAddress.Any, uri.Port, listenOptions =>
            //            {
            //                // Http3 is in preview now
            //                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            //            });
            //        }
            //    };
            //});
            webBuilder.UseStartup<Startup>();
        });

public partial class Program
{
    private static readonly string? Namespace = typeof(Startup).Namespace;
    public static readonly string ApplicationName = Namespace?.Substring(Namespace.IndexOf('.') + 1) ?? "DefaultApplicationName";
    public static readonly string SystemName = ApplicationName.Split(".").FirstOrDefault() ?? "PDEAU";
}
