using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service
{

    public class FileContentResult : ServiceResult
    {
        /// <summary>
        /// The content of the file
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The file encoding. E.g. UTF-8
        /// </summary>
        public string ContentEncoding { get; set; }
        
        /// <summary>
        /// The number of lines in the content
        /// </summary>
        public long RecordsCount { get; set; }
        
        /// <summary>
        /// The file metadata
        /// </summary>
        public AuditLogFileMetaData Metadata { get; set; }

    }
}
