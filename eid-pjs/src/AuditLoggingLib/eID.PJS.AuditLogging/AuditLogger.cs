using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using Serilog.Settings.Configuration;

namespace eID.PJS.AuditLogging
{
    public class AuditLogger : IAuditLogger
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _auditLog;
        private ICryptoKeyProvider _keyProvider;
        private readonly byte[] _encryptionKey;
        private const string CONFIG_SECTION_NAME = nameof(AuditLogger);
        private readonly bool _useLocalTime;
        private static object _lockAuditObject = new object();
        private static object _lockModuleId = new object();
        private readonly string? _systemId;
        private const string EID_SYSTEM_ID = "EID_SYSTEM_ID";

        private static volatile string? _moduleId;
        public static string? ModuleId
        {
            get { return _moduleId; }
            set
            {
                lock (_lockModuleId)
                {
                    _moduleId = value;
                }
            }
        }

#if TEST_MODE_ENABLED
        private bool _testMode;
#endif
        public AuditLogger(IConfiguration configuration, ICryptoKeyProvider keyProvider)
        {
            _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _encryptionKey = _keyProvider.GetKey();

            if (_encryptionKey == null || _encryptionKey.Length == 0)
                throw new ArgumentNullException(nameof(keyProvider));


            var section = configuration.GetSection(CONFIG_SECTION_NAME);
            _useLocalTime = section.GetValue<bool>("useLocalTime");
            _systemId = section.GetValue<string>("systemId")?.ToLower();

            //If the SystemID is not empty then clean it first.
            if (string.IsNullOrWhiteSpace(_systemId))
            {
                var envId = Environment.GetEnvironmentVariable(EID_SYSTEM_ID);

                if (string.IsNullOrWhiteSpace(envId))
                    throw new ConfigurationErrorsException("System ID is not set in configuration parameter 'systemId' or as environment variable in 'EID_SYSTEM_ID'!");

                // Do not override back the System ID to the environment variable. 
                // we do not want to change the environment silently but we'll use only a valid System ID.
                _systemId = AuditLogDIExtensions.SanitizeSystemId(envId);

                if (!AuditLogDIExtensions.IsValidSystemId(envId))
                {
                    Log.Warning("The provided System ID in the environment variable 'EID_SYSTEM_ID' contains invalid characters: '{invalid}'. Using cleaned version: {cleaned}", envId, _systemId);
                }
            }
            else
            {
                // Notify that we are using System ID from the configuration

                var cleanedSystemId = AuditLogDIExtensions.SanitizeSystemId(_systemId);

                if (!AuditLogDIExtensions.IsValidSystemId(_systemId))
                {
                    Log.Warning("The provided System ID from the configuration contains invalid characters: '{invalid}'. Using cleaned version: '{cleaned}' and reseting the 'EID_SYSTEM_ID' environment variable.", _systemId, cleanedSystemId);
                }
                else
                {
                    Log.Information("Using the value from the configuration for the System ID '{systemid}' and reseting the 'EID_SYSTEM_ID' environment variable.", cleanedSystemId);
                }

                Environment.SetEnvironmentVariable("EID_SYSTEM_ID", cleanedSystemId);
            }

            lock (_lockModuleId)
            {
                if (string.IsNullOrWhiteSpace(ModuleId))
                    throw new ConfigurationErrorsException("Module ID is not set before creating the AuditLogger!");
            }

#if TEST_MODE_ENABLED
            _testMode = section.GetValue<bool>("testMode");
#endif

            var opt = new ConfigurationReaderOptions()
            {
                SectionName = CONFIG_SECTION_NAME,
            };

            _auditLog = new LoggerConfiguration()
               .ReadFrom
               .Configuration(configuration, opt)
               .CreateLogger();
        }

        public void LogEvent(AuditLogEvent data)
        {
            lock (_lockAuditObject)
            {

                if (string.IsNullOrWhiteSpace(data.EventType))
                    throw new ArgumentNullException(nameof(data.EventType));

                var eventDate = _useLocalTime ? DateTime.Now : DateTime.UtcNow;

#if TEST_MODE_ENABLED
                if (_testMode)
                    eventDate = eventDate.AddDays(new Random().Next(-31, 32))
                                         .AddHours(new Random().Next(-12, 12))
                                         .AddMinutes(new Random().Next(-30, 30));
#endif

                var record = new AuditLogRecord
                {
                    EventId = Guid.NewGuid().ToString("N"),
                    EventDate = eventDate,
                    EventType = data.EventType,
                    Message = data.Message,
                    SystemId = _systemId,
                    SourceFile = null,
                    EventPayload = data.EventPayloadEncrypted,
                    CorrelationId = data.CorrelationId,
                    ModuleId = ModuleId,
                    RequesterSystemId = data.RequesterSystemId,
                    RequesterSystemName = data.RequesterSystemName,
                    RequesterUserId = data.RequesterUserIdEncrypted,
                    TargetUserId = data.TargetUserIdEncrypted,
                };

                CalculateChecksum(record, _encryptionKey);

                WriteToJson(record);
            }
        }

        public string? SystemId => _systemId;
        private void CalculateChecksum(AuditLogRecord record, byte[] encryptionKey)
        {
            using (var hash = new HMACSHA512(encryptionKey))
            {
                BinaryFormatter serializer = new BinaryFormatter();

                var hashString = JsonConvert.SerializeObject(record, Formatting.None);

                var data = Encoding.UTF8.GetBytes(hashString);
                var checksum = hash.ComputeHash(data);
                record.Checksum = Convert.ToBase64String(checksum);
            }
        }

        private void WriteToJson(AuditLogRecord record)
        {
            var message = JsonConvert.SerializeObject(record, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,

            });
            _auditLog.Information("{Message:l}", message);
            Log.Debug("Stored log message");
        }



    }
}
