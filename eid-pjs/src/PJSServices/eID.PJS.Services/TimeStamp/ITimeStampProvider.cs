using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.TimeStamp
{
    public interface ITimeStampProvider
    {
        Task<TimeStampTokenResult> RequestToken(string auditLogFileHash);
        TimeStampVerificationResult VerifyToken(string auditLogFileHash, string encodedTokenResponse, X509Certificate2 signServerCertChain);
    }
}
