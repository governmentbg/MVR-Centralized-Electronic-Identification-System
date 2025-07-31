using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable disable
namespace eID.PJS.AuditLogging
{
    public class AuditLogRecord: AuditLogEvent
    {

        [JsonProperty("eventId", Order = 1)]
        public string EventId { get; set; }

        [JsonProperty("systemId", Order = 2)]
        public string SystemId { get; set; }
        
        [JsonProperty("eventDate", Order = 3)]
        public DateTime EventDate { get; set; }
        
        [JsonProperty("checksum", Order = 4)]
        public string Checksum { get; set; }

        [JsonProperty("sourceFile", Order = 5)]
        public string SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the module identifier.
        /// </summary>
        /// <value>
        /// The module identifier.
        /// </value>
        [JsonProperty("moduleId", Order = 13)]
        public string ModuleId { get; set; }
    }
}
