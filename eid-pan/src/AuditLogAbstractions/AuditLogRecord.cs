using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable disable
namespace AuditLogAbstractions
{
    public class AuditLogRecord: AuditLogEvent
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }
        
        [JsonProperty("event_date")]
        public DateTime EventDate { get; set; }
        
        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("file_source")]
        public string FileSource { get; set; }
    }
}