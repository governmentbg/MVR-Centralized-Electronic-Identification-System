using System;
using System.Collections.Generic;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.DataContracts;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Logstash logger for adapters
    /// </summary>
    public interface IAdapterLogger
    {
        /// <summary>
        /// Log adapter log record
        /// </summary>
        /// <param name="operationFullName">Operation full name</param>
        /// <param name="logRecord">Log record</param>
        void LogRecord(string operationFullName, AdapterLogRecordType logRecord);
    }
}
