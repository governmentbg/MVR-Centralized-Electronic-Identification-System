using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.AuditLogging
{
    public class AuditChecksumRecord
    {
        [JsonProperty("checksum")]
        public string Checksum { get; set; }
        
        [JsonProperty("sourceFile")]
        public string SourceFile { get; set; }

        [JsonProperty("sourcePath")]
        public string SourcePath { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("systemId")]
        public string SystemId { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// List of events in comma separated format.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        [JsonProperty("events")]
        public string Events { get; set; }

    }
}
