


using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Unicode;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog.Sinks.File.Archive;
using Serilog;


namespace AuditLogAbstractions
{
    public class AuditLogger
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _auditLog;
        private ICryptoKeyProvider _keyProvider;
        private readonly byte[] _encryptionKey;
        private const string CONFIG_SECTION_NAME = "AuditLogger";
        private readonly bool _useLocalTime;
        private static object _lockObject = new object();

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

#if TEST_MODE_ENABLED
            _testMode = section.GetValue<bool>("testMode");
#endif

            _auditLog = new LoggerConfiguration()
               .ReadFrom
               .Configuration(configuration, CONFIG_SECTION_NAME)
               .CreateLogger();
        }

        public void LogEvent(AuditLogEvent data)
        {
            lock (_lockObject)
            {

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
                    SystemId = data.SystemId.ToLower(),
                    FileSource = null,
                    EventPayload = data.EventPayload
                };

                CalculateChecksum(record, _encryptionKey);

                WriteToJson(record);
            }
        }

        private void CalculateChecksum(AuditLogRecord record, byte[] encryptionKey)
        {
            var hash = new HMACSHA512(encryptionKey);

            var hashString = JsonConvert.SerializeObject(record, Formatting.None);

            var data = Encoding.UTF8.GetBytes(hashString);
            var checksum = hash.ComputeHash(data);
            record.Checksum = Convert.ToBase64String(checksum);
        }

        private void WriteToJson(AuditLogRecord record)
        {
            var message = JsonConvert.SerializeObject(record, Formatting.None);
            _auditLog.Information("{Message:l}", message);
        }

    }
}
