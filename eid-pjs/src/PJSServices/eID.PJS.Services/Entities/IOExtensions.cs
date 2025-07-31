#nullable disable

namespace eID.PJS.Services
{
    public static class IOExtensions
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => allowedExtensions.Contains(f.Extension));
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, string extensions)
        {
            if (string.IsNullOrWhiteSpace(extensions))
            {
                return GetFilesByExtensions(dir, "*.*");
            }

            List<string> exts;
            exts = extensions.Split(';', ',', '|').ToList();
            exts.ForEach(f => f = f.Trim());

            return GetFilesByExtensions(dir, exts.ToArray());
        }

        public static string GetRollingFileNameFromPath(string source)
        {

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");

            return Path.Combine(Path.GetDirectoryName(source),
                                        Path.GetFileNameWithoutExtension(source)
                                        + "_"
                                        + DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                                        + Path.GetExtension(source));
        }
        public static string GetRollingFileName(string fileName)
        {

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            return Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
        }

        public static IEnumerable<IEnumerable<string>> EnumerateFilesInBatches(string folderPath, string extensionFilter, int batchSize)
        {
            var files = Directory.EnumerateFiles(folderPath, extensionFilter, SearchOption.AllDirectories);

            var batch = new List<string>(batchSize);

            foreach (var file in files)
            {
                batch.Add(file);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<string>(batchSize);
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }

        public static IEnumerable<IEnumerable<string>> EnumerateFilesInBatches(string folderPath, string extensionFilter, int batchSize, DateTime startDate, DateTime endDate)
        {
            var files = Directory.EnumerateFiles(folderPath, extensionFilter, SearchOption.AllDirectories);

            var batch = new List<string>(batchSize);

            foreach (var file in files)
            {
                // Get the file's creation date
                var fileInfo = new FileInfo(file);
                var fileCreationDate = fileInfo.CreationTime;

                // Check if the file's creation date is within the specified date range
                if (fileCreationDate >= startDate && fileCreationDate <= endDate)
                {
                    batch.Add(file);

                    if (batch.Count == batchSize)
                    {
                        yield return batch;
                        batch = new List<string>(batchSize);
                    }
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }

        public static IEnumerable<IEnumerable<string>> EnumerateFilesInBatches(string folderPath, string extensionFilter, int batchSize, Func<string, bool> fileFilter)
        {
            var files = Directory.EnumerateFiles(folderPath, extensionFilter, SearchOption.AllDirectories).Where(fileFilter);

            var batch = new List<string>(batchSize);

            foreach (var file in files)
            {
                batch.Add(file);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<string>(batchSize);
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }

        /// <summary>
        /// Determines whether [is sub path] [the specified base path].
        /// Example usage:
        /// string basePath = "C:\\ParentFolder";
        /// string path = "C:\\ParentFolder\\SubFolder\\file.txt";
        /// bool isSubPath = PathHelper.IsSubPath(basePath, path);
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if [is sub path] [the specified base path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSubPath(string basePath, string path)
        {
            // Normalize the paths to a standard format
            var normalizedBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            // Use a case-insensitive comparison for Windows paths
            return normalizedPath.StartsWith(normalizedBasePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if there are missing or empty subfolders for all dates in the selected period
        /// </summary>
        /// <param name="path">i.e. System audit log files path</param>
        /// <param name="startDate"></param>
        /// <param name="toDate">Uses UtcNow.Date When MaxDate is passed</param>
        /// <returns>Dates from the selected period that have no files.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static HashSet<DateTime> GetDatesFromPeriodWithMissingOrEmptyCalendarFolders(string path, DateTime startDate, DateTime toDate)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            var datesWithNoLogs = new HashSet<DateTime>();
            var currentDate = startDate;
            var maxDate = toDate == DateTime.MaxValue ? DateTime.UtcNow.Date : toDate.Date;
            do
            {
                var currentPath = Path.Combine(path, currentDate.Year.ToString("D4"), currentDate.Month.ToString("D2"), currentDate.Day.ToString("D2"));
                if (!Directory.Exists(currentPath) || !Directory.EnumerateFileSystemEntries(currentPath).Any())
                {
                    datesWithNoLogs.Add(currentDate.Date);
                }
                currentDate = currentDate.AddDays(1);
            } while (currentDate.Date <= maxDate.Date);
            return datesWithNoLogs;
        }

        /// <summary>
        /// Gets the directory with the longest path, ordered by name (presumably oldest date value).
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns parsed date or UtcNow when unable to parse the date or no folder is found.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static DateTime GetOldestNonEmptyCalendarFolderDateInPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            var oldestDirectory = new DirectoryInfo(path)
                                        .GetDirectories("*", SearchOption.AllDirectories)
                                        .Where(d => d.Exists && d.GetFiles().Any())
                                        .OrderByDescending(
                                            p => p.FullName.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar))
                                        .ThenBy(p => p.FullName)
                                        .FirstOrDefault();
            if (oldestDirectory == null)
            {
                return DateTime.UtcNow;
            }
            // if we're at the right spot
            // Parent.Parent is the year
            // Parent - Month
            // oldestDirectory is the day
            var oldestDateString = $"{oldestDirectory.Parent.Parent.Name} {oldestDirectory.Parent.Name} {oldestDirectory.Name}";
            var format = "yyyy MM dd";
            if (!DateTime.TryParseExact(oldestDateString,
                                        format,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None,
                                        out var parsedDate))
            {
                return DateTime.UtcNow;
            }

            return parsedDate;
        }
    }
}
