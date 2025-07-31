using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using eID.PJS.AuditLogging;
using System.Globalization;
using System.Dynamic;

#nullable disable

namespace AuditLogSourceApp
{
    public class AuditLogGeneratorService : IHostedService
    {
        private readonly AuditLogger _logger;
        private Timer _timer;
        private readonly string[] _commands = { "INSERT", "UPDATE", "DELETE", "READ", "LOGIN", "LOGOUT" };
        private readonly string[] _systems = { "eid-pan", "eid-pun", "eid-iscei", "eid-mpozei", "eid-pg", "eid-rei", "eid-ruei" };
        private readonly Guid _correlationId = Guid.NewGuid();
        private readonly List<string> _fieldNames = new List<string>();
        public AuditLogGeneratorService(AuditLogger logger)
        {
            _logger = logger;

            for (int i = 0; i < 10; i++)
            {
                _fieldNames.Add(RandomString((new Random()).Next(4, 10), false));
            }
        }

        private void ExecuteTimer(object state)
        {
            GenerateLogEntry();
        }

        private void GenerateLogEntry()
        {

            var range = new Random().Next(2, 50);

            Parallel.For(1, range, index =>
            {
                var reqSystemId = _systems[(new Random()).Next(0, 7)];

                _logger.LogEvent(new AuditLogEvent
                {
                    Message = $"Това съобщение номер {index} е генерирано на {DateTime.Now} \r\n Втори ред текст. \r\n",
                    EventType = _commands[(new Random()).Next(0, 6)],
                    CorrelationId = _correlationId.ToString(),
                    EventPayload = GenerateDictionaryPayload(),
                    RequesterSystemId = reqSystemId,
                    RequesterSystemName = $"System {reqSystemId}",
                    RequesterUserId = $"user-{(new Random()).Next(0, 9)}",
                    TargetUserId = $"user-{(new Random()).Next(0, 9)}",
                });
            });
        }

        private object GenerateAnonymousPayload()
        {
            return new
            {
                FieldString = RandomString(20, false),
                FieldGuid = Guid.NewGuid(),
                FieldDate = DateTime.UtcNow.AddDays(-(new Random()).Next(0, 100)),
                FieldNum = (new Random()).Next(1000, 999_999_999)
            };
        }

        private object GeneratePayloadTuple()
        {
            return new Tuple<string, Guid, DateTime, int>(
                RandomString(20, false),
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(-(new Random()).Next(0, 100)),
                (new Random()).Next(1000, 999_999_999));
        }

        private SortedDictionary<string, object> GenerateDictionaryPayload()
        {

            var data = new SortedDictionary<string, object>();
            int numFields = (new Random()).Next(0, 10);

            for (int i = 0; i <= numFields; i++)
            {
                switch (i)
                {
                    case >= 0 and < 3: // string
                        data.Add(_fieldNames[i], RandomString(i + 10, false));
                        break;
                    case >= 3 and < 6: // DateTime
                        data.Add(_fieldNames[i], DateTime.UtcNow.AddDays(-i));
                        break;
                    case >= 6 and < 8: // GUID
                        data.Add(_fieldNames[i], Guid.NewGuid());
                        break;
                    case >= 8: // Number
                        data.Add(_fieldNames[i], (new Random()).Next(1000, 999_999_999));
                        break;
                }
            }

            return data;
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();

            Random random = new Random();

            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (lowerCase)
                return builder.ToString().ToLower(CultureInfo.InvariantCulture);

            return builder.ToString();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogEvent(new AuditLogEvent
            {
                Message = $"Одит лог генератор услугата е стартирана",
                EventType = "START"
            });

            _timer = new Timer(ExecuteTimer, null, 0, 500);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {


            _logger.LogEvent(new AuditLogEvent
            {
                Message = $"Одит лог генератор услугата е спряна",
                EventType = "STOP"
            });

            _timer.Dispose();

            return Task.CompletedTask;
        }
    }
}
