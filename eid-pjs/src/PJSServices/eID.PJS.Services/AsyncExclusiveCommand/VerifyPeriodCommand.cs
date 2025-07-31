using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Verification;

namespace eID.PJS.Services
{
    public class VerifyPeriodCommand : BackgroundTaskCommand<VerificationServiceStateShort>
    {
        private VerificationService _service;
        private VerifyPeriodRequest _request;
        public VerifyPeriodCommand(ICommandStateProvider<VerificationServiceStateShort> stateProvider, VerificationService service, VerifyPeriodRequest request, ILogger logger) : base(stateProvider, logger)
        {
            _service = service;
            _request = request;
        }

        protected override VerificationServiceStateShort ExecuteTask(CancellationToken cancellationToken)
        {
            return _service.VerifyPeriod(_request.SystemId, _request.StartDate, _request.EndDate);
        }
    }
}
