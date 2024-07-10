using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace AuditLogAbstractions
{
    public class AuditChecksumRecord
    {
        [JsonProperty("checksum")]
        public string Checksum { get; set; }
        
        [JsonProperty("source_file")]
        public string SourceFile { get; set; }

        [JsonProperty("source_path")]
        public string SourcePath { get; set; }

        [JsonProperty("date_created")]
        public DateTime DateCreated { get; set; }

    }
}
