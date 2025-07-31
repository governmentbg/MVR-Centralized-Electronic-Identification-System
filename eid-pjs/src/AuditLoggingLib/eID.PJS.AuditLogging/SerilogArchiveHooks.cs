using System.IO.Compression;
using Serilog.Sinks.File.Archive;

#nullable disable

namespace eID.PJS.AuditLogging
{
    public static class SerilogArchiveHooks
    {
        static int _alreadyCalled = 0;
        public static string TargetDir { get; set; }
        public static ArchiveHooks ArchiveHooksConfig { get; set; }

        /// <summary>
        /// Must be called only once during the lifetime of the application!!!
        /// </summary>
        public static void Initialize()
        {
            if (Interlocked.Increment(ref _alreadyCalled) == 1)
            {
                ArchiveHooksConfig = new ArchiveHooks(CompressionLevel.NoCompression, TargetDir);
            }
        }
    }
}
