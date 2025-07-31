using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.TimeStamp
{
    public class TimeStampVerificationResult
    {
        public bool IsValid { get; set; }

        public Rfc3161TimestampTokenInfo TokenInfo { get; set; }
        public Exception? Error { get; set; }
    }
}
