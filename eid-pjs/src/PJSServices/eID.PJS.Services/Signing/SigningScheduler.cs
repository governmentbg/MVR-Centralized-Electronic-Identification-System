using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.OpenSearch;

using Microsoft.Extensions.DependencyInjection;

namespace eID.PJS.Services.Signing
{
    public class SigningScheduler : ServiceSchedulerBase<SigningScheduler, SigningSchedulerSettings, SigningServiceState>
    {
        public SigningScheduler(IServiceProvider services, ILogger<SigningScheduler> logger, SigningSchedulerSettings settings, GlobalStatus status) : base(services, logger, settings, status)
        {
        }

        protected override void UpdateState(SigningServiceState? state)
        {
            lock (_lock)
            {
                _status.SigningServiceStatus.LastState = state;
            }
        }

        protected override void UpdateWorkingStatus(WorkingStatus status)
        {
            lock (_lock)
            {
                if (status == WorkingStatus.Processing)
                    _status.SigningServiceStatus.LastProcessingStart = DateTime.UtcNow;

                _status.SigningServiceStatus.CurrentStatus = status;
            }
        }
    }
}
