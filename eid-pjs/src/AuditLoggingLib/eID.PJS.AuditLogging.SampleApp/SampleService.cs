using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.AuditLogging.SampleApp
{
    public class SampleService : IHostedService
    {
        private readonly AuditLogger _auditLogger;

        public SampleService(AuditLogger logger)
        {
            _auditLogger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _auditLogger.LogEvent(new AuditLogEvent
            {
                Message = $"Одит лог примерно приложение е стартирано.",
                EventType = "START"
            });

            await DoSomeWork();

        }

        public async Task DoSomeWork()
        {

            // DO SOME WORK THEN LOG THE AUDIT EVENT!
            _auditLogger.LogEvent(new AuditLogEvent
            {
                Message = $"Извършена е примерна работа.",
                EventType = "SampleApp.SomeEvent",
                EventPayload = new SortedDictionary<string, object> { { "UserId" , "SOME_SUER" }, { "SomeData", "Some Data" } },
                CorrelationId = Guid.NewGuid().ToString(),
                RequesterSystemId = "eid-sample-app",
                RequesterSystemName = "eID Sample App",
                RequesterUserId = $"user-{(new Random()).Next(0, 9)}",
                TargetUserId = $"user-{(new Random()).Next(0, 9)}",
            });



            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _auditLogger.LogEvent(new AuditLogEvent
            {
                Message = $"Одит лог примерно приложение е спряно.",
                EventType = "STOP"
            });

            return Task.CompletedTask;
        }
       
    }
}
