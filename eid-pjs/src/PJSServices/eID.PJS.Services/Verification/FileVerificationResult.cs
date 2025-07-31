using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eID.PJS.Services.Verification;

#nullable disable
public class FileVerificationResult
{
    public string SourceFile { get; set; }
    public List<RecordVerificationResult> InvalidRecords { get; set; } = new List<RecordVerificationResult>();
    public List<string> Errors { get; set; } = new List<string> { };

    public bool IsValid
    {
        get 
        {
            return InvalidRecords.All(r => r.IsValid) && !Errors.Any();
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; }
}

