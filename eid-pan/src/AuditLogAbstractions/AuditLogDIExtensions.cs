using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Steeltoe.Extensions.Configuration.RandomValue;
using Steeltoe.Extensions.Configuration.Placeholder;

using AuditLogAbstractions;

namespace AuditLogAbstractions
{
    public static class AuditLogDIExtensions
    {
        public static IHostBuilder EnableSerilogSelfLog(this IHostBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Out);
            return builder;
        }

        public static void AddAuditLog(this IServiceCollection svc, IConfiguration config)
        {
            var section = config.GetSection(nameof(SerilogArchiveHooks));
            SerilogArchiveHooks.TargetDir = section.GetValue<string>("TargetDir");
            SerilogArchiveHooks.Initialize();

            svc.AddSingleton<ICryptoKeyProvider, ConfigurationCryptoKeyProvider>();
            svc.AddSingleton<AuditLogger>();
        }

        public static IConfigurationBuilder AddConfigurationPreprocessing(this IConfigurationBuilder builder)
        {
            builder.AddRandomValueSource();
            builder.AddPlaceholderResolver();

            return builder;
        }
    }
}
