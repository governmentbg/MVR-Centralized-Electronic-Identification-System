using eID.RO.Application;
using eID.RO.Application.StateMachines;
using eID.RO.Service.Database;
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

    MigrateDbContext(host);
    MigrateSagasDbContext(host);

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
                .Enrich.WithProperty("SystemId", ApplicationName);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

static void MigrateDbContext(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

    AddInformation($"Migrating database associated with context {nameof(ApplicationDbContext)}");

    context.Database.Migrate();

    context.Seed(logger);
}

static void MigrateSagasDbContext(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    AddInformation($"Migrating database associated with context {nameof(SagasDbContext)}");
    services.GetRequiredService<SagasDbContext>().Database.Migrate();
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
    private static readonly string? _namespace = typeof(Startup).Namespace;
    public static readonly string ApplicationName = _namespace?.Substring(_namespace.LastIndexOf('.', _namespace.LastIndexOf('.') - 1) + 1) ?? "DefaultApplicationName";
}
