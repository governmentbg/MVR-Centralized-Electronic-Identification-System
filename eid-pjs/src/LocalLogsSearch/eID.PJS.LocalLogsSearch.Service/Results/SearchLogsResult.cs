using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service
{
    public class SearchLogsResult : ServiceResult
    {
        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <value>
        /// The records count.
        /// </value>
        public int RecordsCount => Records.Count;

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public JArray Records { get; set; } = new JArray();

        public bool ResultLimitReached { get; set; }
        public int ConfiguredLimit { get; set; }
    }
}
