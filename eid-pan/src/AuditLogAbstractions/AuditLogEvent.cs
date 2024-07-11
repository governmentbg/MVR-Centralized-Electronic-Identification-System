using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable disable

namespace AuditLogAbstractions
{
    public class AuditLogEvent
    {
        [JsonProperty("system_id")]
        public string SystemId { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("event_payload")]
        public object EventPayload { get; set; }
    }
}