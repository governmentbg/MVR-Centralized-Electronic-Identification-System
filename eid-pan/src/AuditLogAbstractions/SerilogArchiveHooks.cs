using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog.Sinks.File.Archive;

#nullable disable

namespace AuditLogAbstractions
{
    public static class SerilogArchiveHooks
    {
        public static string TargetDir { get; set; }
        public static ArchiveHooks ArchiveHooksConfig { get; set; }

        /// <summary>
        /// Must be called only once during the lifetime of the application!!!
        /// </summary>
        public static void Initialize()
        {
            ArchiveHooksConfig = new ArchiveHooks(CompressionLevel.NoCompression, TargetDir);
        }
    }
}
