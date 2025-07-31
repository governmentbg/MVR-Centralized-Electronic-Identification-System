using eID.MIS.Application;
using eID.MIS.Service.Database;
using eID.PJS.AuditLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

Console.Title = ApplicationName;
Console.WriteLine($"({ApplicationName}) is starting...");

try
{
    var hostBuilder = CreateHostBuilder(args);

    Console.WriteLine($"Building host ({ApplicationName})...");

    var host = hostBuilder.Build();

    AddInformation($"Applying migrations ({ApplicationName})...");

    MigratePaymentsDbContext(host);
    MigrateDeliveriesDbContext(host);

    AddInformation($"Starting host ({ApplicationName})...");

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
            webBuilder.UseStartup<Startup>();
        });


static void MigratePaymentsDbContext(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PaymentsDbContext>();
    var logger = services.GetRequiredService<ILogger<PaymentsDbContext>>();

    AddInformation($"Migrating database associated with context {nameof(PaymentsDbContext)}");

    context.Database.Migrate();
}

static void MigrateDeliveriesDbContext(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DeliveriesDbContext>();
    var logger = services.GetRequiredService<ILogger<DeliveriesDbContext>>();

    AddInformation($"Migrating database associated with context {nameof(DeliveriesDbContext)}");

    context.Database.Migrate();
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

public partial class Program
{
    private static readonly string? Namespace = typeof(Startup).Namespace;
    public static readonly string ApplicationName = Namespace?.Substring(Namespace.IndexOf('.') + 1) ?? "DefaultApplicationName";
    public static readonly string SystemName = ApplicationName.Split(".").FirstOrDefault() ?? "MIS";
}
