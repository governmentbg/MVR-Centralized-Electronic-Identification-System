using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace eID.PJS.LocalLogsSearch.Service
{
    public static class SystemExtensions
    {
        /// <summary>
        /// Suggest the maximum degree of parallelism that can be safely used.
        /// </summary>
        /// <returns>Returns the number of cores/vCPUs that can be used</returns>
        public static int SuggestMaxDegreeOfParallelism()
        {
            int suggest = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));

            if (suggest == 0)
                return 1;


            if (OperatingSystem.IsLinux() && suggest == 1)
            {
                suggest = GetLinuxTotalProcessorCount();
            }

            if (suggest == 0)
                return 1;

            return suggest;
        }

        /// <summary>
        /// Sets the desired processor count for linux/container environment.
        /// </summary>
        /// <param name="processorCnt">The processor count.</param>
        /// <returns></returns>
        public static string? SetLinuxProcessorCount(int processorCnt)
        {
            //export DOTNET_PROCESSOR_COUNT=$(nproc --all)

            if (!OperatingSystem.IsLinux())
                return null;

            if (processorCnt < 0)
                return "Invalid processor count";

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Set the total processor count
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    if (processorCnt == 0)
                    {
                        sw.WriteLine("export DOTNET_PROCESSOR_COUNT=$(nproc --all)");
                    }
                    else
                    {
                        sw.WriteLine($"export DOTNET_PROCESSOR_COUNT={processorCnt}");
                    }
                    sw.Close();
                }
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                return error;
            }

            return null;
        }

        public static int GetLinuxTotalProcessorCount()
        {

            //export DOTNET_PROCESSOR_COUNT=$(nproc --all)

            if (!OperatingSystem.IsLinux())
                return 0;

            int totalProcessorCount = 0;

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Execute the 'nproc --all' command to get the total processor count
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("nproc --all");
                    sw.Close();
                }
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (string.IsNullOrWhiteSpace(error))
            {
                if (int.TryParse(output, out totalProcessorCount))
                {
                    return totalProcessorCount;
                }
            }

            return totalProcessorCount;
        }

        /// <summary>
        /// Measures performance counters like memory usage and time for completion for an arbitrary peace of code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeToMeasure">The code to measure</param>
        /// <param name="result">The result object where to put the measurements. Must implement IPerformanceMetrics</param>
        public static void MeasureCodeExecution<T>(Action codeToMeasure, T result) where T : IPerformanceMetrics
        {
            Stopwatch sw = Stopwatch.StartNew();

            result.Metrics.StartDate = DateTime.UtcNow;
            result.Metrics.ThreadId = Thread.CurrentThread.ManagedThreadId;

            codeToMeasure.Invoke();

            var process = Process.GetCurrentProcess();

            result.Metrics.PrivateMemorySize = process.PrivateMemorySize64;
            result.Metrics.PagedMemorySize = process.PagedMemorySize64;
            result.Metrics.VirtualMemorySize = process.VirtualMemorySize64;
            result.Metrics.GCMemorySize = GC.GetTotalMemory(true);

            sw.Stop();

            result.Metrics.ProcessingTime = sw.Elapsed;
            result.Metrics.EndDate = result.Metrics.StartDate.AddMilliseconds(sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Counts the number of lines in a string.
        /// It takes into account only th LF in this way it works for Windows and Linux line endings.
        /// </summary>
        /// <param name="s">The string to count the lines of</param>
        /// <returns>The number of lines in the string</returns>
        public static long CountLines(this string s)
        {
            long count = 1;
            int position = 0;
            while ((position = s.IndexOf("\n", position)) != -1)
            {
                count++;
                position++;         // Skip this occurrence!
            }
            return count;
        }


        public static string? EnsureValidProcessorCountInLinux(HostBuilderContext ctx)
        {
            if (OperatingSystem.IsLinux())
            {
                var currentProcCount = GetLinuxTotalProcessorCount();
                var configProcCount = ctx.Configuration.GetValue<int>("ProcessorCount");

                if (currentProcCount < 2 && configProcCount > 0)
                {
                    return SetLinuxProcessorCount(configProcCount);
                }
            }

            return null;
        }
    }


}
