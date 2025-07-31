using eID.PJS.Services.Archiving;
using eID.PJS.Services.Signing;
using eID.PJS.Services.Verification;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    /// <summary>
    /// Class that holds the status of the application.
    /// THE CLASS IS NOT THREAD SAFE AND IS USED AS SINGLETON OF A GLOBAL STATE AND MUST BE MODIFIED ONLY WITH LOCKING!!!
    /// </summary>
    public class GlobalStatus
    {
        private readonly SigningServiceStatus _signingServiceStatus = new SigningServiceStatus() { ServiceName = "SigningService" };
        public SigningServiceStatus SigningServiceStatus
        {
            get
            {
                return _signingServiceStatus;
            }
        }

        private readonly VerificationServiceStatus _verificationServiceStatus = new VerificationServiceStatus() { ServiceName = "VerificationService" };
        public VerificationServiceStatus VerificationServiceStatus
        {
            get
            {
                return _verificationServiceStatus;
            }
        }
    }

    public enum WorkingStatus
    {
        NotRunning = 0,
        Ready = 1,
        Processing = 2,
        Error = 3
    }
}
