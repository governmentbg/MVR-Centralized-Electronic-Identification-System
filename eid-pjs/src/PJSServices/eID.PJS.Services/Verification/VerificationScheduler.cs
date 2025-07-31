using eID.PJS.Services.OpenSearch;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Verification;

public class VerificationScheduler : ServiceSchedulerBase<VerificationScheduler, VerificationSchedulerSettings, VerificationServiceStateShort>
{
    public VerificationScheduler(IServiceProvider services, ILogger<VerificationScheduler> logger, VerificationSchedulerSettings settings, GlobalStatus status) : base(services, logger, settings, status)
    {
    }

    protected override void UpdateState(VerificationServiceStateShort? state)
    {
        lock (_lock)
        {
            _status.VerificationServiceStatus.LastState = state;
        }
    }

    protected override void UpdateWorkingStatus(WorkingStatus status)
    {
        lock (_lock)
        {
            if (status == WorkingStatus.Processing)
                _status.VerificationServiceStatus.LastProcessingStart = DateTime.UtcNow;

            _status.VerificationServiceStatus.CurrentStatus = status;
        }
    }
}

