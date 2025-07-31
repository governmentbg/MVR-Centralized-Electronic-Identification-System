using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.LocalLogsSearch.Service
{
    public class FileListResult : ServiceResult
    {
        /// <summary>Gets the records count.</summary>
        /// <value>The records count.</value>
        public int RecordsCount => Records.Count;
        /// <summary>Gets or sets the records.</summary>
        /// <value>The records.</value>
        public List<AuditLogFileMetaData> Records { get; set; } = new List<AuditLogFileMetaData>();

    }
}
