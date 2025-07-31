using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Verification;

#nullable disable
public class RecordVerificationResult
{
    public string EventId { get; set; }
    public bool IsChecksumMatch { get; set; }
    public bool RecordMatchLocalLogs { get; set; }
    public bool LocalLogRecordExists { get; set; }

    public bool IsValid 
    {
        get { return IsChecksumMatch && RecordMatchLocalLogs && LocalLogRecordExists; }
    }
}

