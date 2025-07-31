using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.Placeholder;
using Steeltoe.Extensions.Configuration.RandomValue;

#nullable disable

namespace eID.PJS.AuditLogging
{
    public static class AuditLogDIExtensions
    {
        private static object _lockObj = new object();
        private static string _configuredSystemId;
        private static string _cleanedSystemId;

        public static string ConfiguredSystemId { get => _configuredSystemId; }
        public static string CleanedSystemId { get => _cleanedSystemId; }

        /// <summary>
        /// As the audit logging library will always try to use a cleaned/correct value for System ID this can have side effects and
        /// it is a good idea to log that the system is using not what is provided as a System ID.
        /// Log if the passed value to the AddAuditLogConfiguration is invalid and a cleaned value is used instead.
        /// </summary>
        public static void LogIfInvalidSystemIdIsProvided()
        {
            if (!_configuredSystemId.Equals(_cleanedSystemId))
                Serilog.Log.Warning("Invalid System ID is provided in AddAuditLogConfiguration. Provided value: '{invalid}', Cleaned value: '{cleaned}'", _configuredSystemId, _cleanedSystemId);
        }

        public static IHostBuilder EnableSerilogSelfLog(this IHostBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Out);

            return builder;
        }

        /// <summary>
        /// Registers the audit log service in the DI.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        /// <param name="config">The configuration.</param>
        public static void AddAuditLog(this IServiceCollection svc, IConfiguration config)
        {
            SetSerilogArchiveHooks(config);

            svc.AddSingleton(svc =>
            {
                if (config == null) throw new ArgumentNullException(nameof(ICryptoKeyProvider));

                var pType = Type.GetType(config.GetSection(nameof(ICryptoKeyProvider)).GetValue<string>("Type"));

                if (pType == null)
                    throw new ArgumentNullException("Cannot get the type for the ICryptoKeyProvider");

                return (ICryptoKeyProvider)Activator.CreateInstance(pType, config);
            });

            EncryptionHelper.SetEncryptionKey(config.GetSection(nameof(AuditLogger)).GetValue<string>("EncryptionKey"));
            svc.AddSingleton<AuditLogger>();
            svc.AddSingleton<IAuditLogger>(provider => provider.GetRequiredService<AuditLogger>());
        }

        /// <summary>
        /// Sets the serilog archive hooks for the acrhiving process of the audit logs to be able to work.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void SetSerilogArchiveHooks(IConfiguration config)
        {
            var section = config.GetSection(nameof(SerilogArchiveHooks));
            SerilogArchiveHooks.TargetDir = section.GetValue<string>("TargetDir");
            SerilogArchiveHooks.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="systemId">
        /// System ID that will be logged with the audit logs. Usually your application name.
        /// SystemId will participate in the name of the log file and OpenSearch iondexes so do not use any non-alphanumeric characters except the dash ("-") symbol!!!
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IConfigurationBuilder AddAuditLogConfiguration(this IConfigurationBuilder builder, string systemId, string moduleId)
        {

            if (string.IsNullOrWhiteSpace(systemId))
            {
                throw new ArgumentNullException($"Parameter {nameof(systemId)} is required to build the audit log configuration!");
            }

            if (string.IsNullOrWhiteSpace(moduleId))
            {
                throw new ArgumentNullException($"Parameter {nameof(moduleId)} is required to build the audit log configuration!");
            }

            AuditLogger.ModuleId = moduleId;

            var cleanedSystemId = SanitizeSystemId(systemId);

            if (!AuditLogDIExtensions.IsValidSystemId(systemId))
            {
                lock (_lockObj)
                {
                    _cleanedSystemId = cleanedSystemId;
                    _configuredSystemId = systemId;
                }
            }

            Environment.SetEnvironmentVariable("EID_SYSTEM_ID", cleanedSystemId);
            builder.AddEnvironmentVariables()
                   .AddRandomValueSource()
                   .AddPlaceholderResolver();



            return builder;
        }

        /// <summary>
        /// Cleans any non-alphanumeric characters except dash in the provided value and replaces it with dash.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Cleaned value</returns>
        public static string SanitizeSystemId(string value)
        {
            // Replace non-alphanumeric characters with dash
            value = Regex.Replace(value, "[^a-zA-Z0-9]", "-");

            // Replace consecutive dashes with a single dash
            while (value.Contains("--"))
            {
                value = value.Replace("--", "-");
            }

            return value.ToLower();
        }

        /// <summary>
        /// Determines whether the provided value is a valid System ID.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if [is valid system identifier] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidSystemId(string input)
        {
            // Check for non-alphanumeric characters except dash
            return !Regex.IsMatch(input, "[^a-zA-Z0-9-]");
        }
    }
}
