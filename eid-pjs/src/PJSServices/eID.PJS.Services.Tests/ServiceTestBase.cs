using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Steeltoe.Extensions.Configuration.Placeholder;
using Steeltoe.Extensions.Configuration.RandomValue;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Tests
{
    public abstract class ServiceTestBase
    {
        protected static IConfiguration? _config;
        protected static IServiceCollection _services = new ServiceCollection();
        protected ServiceTestBase()
        {

            _config = BuildConfiguration();
        }

        protected IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Development.json", optional: true)
                        .AddEnvironmentVariables()
                        .AddRandomValueSource()
                        .AddPlaceholderResolver()
                        .Build();
        }

    }
}
