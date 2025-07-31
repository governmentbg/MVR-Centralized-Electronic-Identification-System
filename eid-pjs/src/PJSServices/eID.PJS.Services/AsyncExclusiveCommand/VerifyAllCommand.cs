using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Verification;

namespace eID.PJS.Services
{
    public class VerifyAllCommand : BackgroundTaskCommand<VerificationServiceStateShort>
    {
        private VerificationService _service;
        public VerifyAllCommand(ICommandStateProvider<VerificationServiceStateShort> stateProvider, VerificationService service, ILogger logger) : base(stateProvider, logger)
        {
            _service = service;
        }

        protected override VerificationServiceStateShort ExecuteTask(CancellationToken cancellationToken)
        {
            return _service.VerifyAll();
        }
    }
}
