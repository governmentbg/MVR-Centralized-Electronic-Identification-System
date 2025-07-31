using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.Placeholder;
using Steeltoe.Extensions.Configuration.RandomValue;

namespace eID.PJS.LocalLogsSearch.Tests;

#nullable disable
public abstract class ServiceTestBase
{
    protected static IConfiguration _config;
    protected ServiceTestBase()
    {

        _config = BuildConfiguration();
    }

    protected IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddRandomValueSource()
                    .AddPlaceholderResolver()
                    .Build();
    }

}
