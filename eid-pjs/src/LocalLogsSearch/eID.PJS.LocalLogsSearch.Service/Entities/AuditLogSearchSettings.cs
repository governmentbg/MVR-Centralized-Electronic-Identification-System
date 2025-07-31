using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service.Entities
{
    public class AuditLogSearchSettings
    {

        /// <summary>
        /// Gets or sets the maximum records.
        /// The maximum records that can be returned from the search query.
        /// </summary>
        /// <value>
        /// The maximum records.
        /// </value>
        public int MaxRecords { get; set; }
    }
}
