using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service
{
    /// <summary>
    /// Represents the audit log file meta data based on the name of the file and its location
    /// </summary>
    public class AuditLogFileMetaData
    {
        [JsonProperty("sourceFile")]
        public string SourceFile { get; set; }

        [JsonProperty("sourceFilePath")]
        public string SourceFilePath { get; set; }

        [JsonProperty("fileTimeStamp")]
        public DateTime FileTimeStamp { get; set; }

        [JsonProperty("fileIndex")]
        public int FileIndex { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

    }
}
