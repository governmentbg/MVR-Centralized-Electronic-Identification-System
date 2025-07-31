using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using eID.PJS.LocalLogsSearch.Service.Entities;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eID.PJS.LocalLogsSearch.Service
{


    /// <summary>
    /// Service that can return list of records located in the
    /// local audit log files.
    /// </summary>
    public class AuditLogsFileService
    {
        /// <summary>
        /// Internal class used to hold the counters
        /// </summary>
        private class Counters
        {
            public int FilesCount;
            public int RecordsCount;
        }

        private ILogger<AuditLogsFileService> _logger;
        private readonly int _maxDegreeOfParallelism;
        private AuditLogSearchSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogsFileService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AuditLogsFileService(ILogger<AuditLogsFileService> logger, AuditLogSearchSettings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maxDegreeOfParallelism = SystemExtensions.SuggestMaxDegreeOfParallelism();
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        /// <summary>
        /// Filters the logs.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Instance of the SearchLogsResult</returns>
        public SearchLogsResult FilterLogs(SearchLogsRequest request)
        {
            var result = new SearchLogsResult();
            bool resultLimitReached = false;


            result.ConfiguredLimit = _settings.MaxRecords;
            var validationResult = request.Validate();

            if (!validationResult.IsValid)
            {
                result.ValidationErrors = validationResult;
                return result;
            }

            var fileItems = new ConcurrentBag<JObject>();
            var parseErrors = new ConcurrentBag<string>();

            SystemExtensions.MeasureCodeExecution(() =>
            {

                foreach (var folder in request.Folders)
                {
                    try
                    {
                        var fullPath = Path.GetFullPath(folder);

                        if (!Directory.Exists(fullPath))
                        {
                            parseErrors.Add($"Folder '{fullPath}' configured for reading does not exists.");
                            _logger.LogWarning("Folder '{Folder}' configured for reading does not exists.", fullPath);
                            continue;
                        }

                        var dir = new DirectoryInfo(fullPath);

                        if (string.IsNullOrWhiteSpace(request.FileExtensionFilter))
                        {
                            request.FileExtensionFilter = "*.log";
                            _logger.LogWarning("fileFilter parameter is empty. Fallback to default: '*.log'");
                        }

                        var files = dir.GetFiles(request.FileExtensionFilter, SearchOption.AllDirectories);

                        var options = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxDegreeOfParallelism
                        };

                        Parallel.ForEach(files, options, (file, c) =>
                        {
                            var errors = ParseFile(file, fileItems, request.FileDateRangeFilter, request.DataQuery);

                            if (errors != null && errors.Any())
                            {
                                errors.ForEach(e => parseErrors.Add(e));
                            }

                            // Exit as soon as possible when the maximum allowed records limit is reached
                            // The actual result count can differ with a big margin depending on the value of the MaxRecords
                            if (_settings.MaxRecords > 0 && fileItems.Count >= _settings.MaxRecords)
                            {
                                // Racing conditions doesen't matter here
                                resultLimitReached = true;
                                c.Stop();
                            }
                        });

                    }
                    catch (Exception ex)
                    {
                        parseErrors.Add($"Error in processing folder '{folder}'. Error: {ex}");
                        _logger.LogError(ex, "Error in processing folder '{Folder}'", folder);
                    }
                }

                result.ResultLimitReached = resultLimitReached;

                fileItems.ToList().ForEach(w => result.Records.Add(w));
                parseErrors.ToList().ForEach(error => result.Errors.Add(error));

                var process = Process.GetCurrentProcess();

                result.Metrics.PrivateMemorySize = process.PrivateMemorySize64;
                result.Metrics.PagedMemorySize = process.PagedMemorySize64;
                result.Metrics.VirtualMemorySize = process.VirtualMemorySize64;
                result.Metrics.GCMemorySize = GC.GetTotalMemory(true);

            }, result);


            return result;
        }
        /// <summary>
        /// Parses the file and adds the extracted records to the provided fileItems collection.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileItems">The file items.</param>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="dataFilter">The data filter.</param>
        /// <param name="parseErrors">The parse errors.</param>
        private List<string> ParseFile(FileInfo file, ConcurrentBag<JObject> fileItems, DateRange fileFilter, QueryNode? dataFilter)
        {
            var parseErrors = new List<string>();

            var (header, metaErrors) = ParseFileMetaData(file, fileFilter);


            if (metaErrors != null && metaErrors.Any())
            {
                parseErrors.AddRange(metaErrors);
            }

            if (header == null)
                return parseErrors;

            int lineNumber = 0;
            foreach (var fileLine in File.ReadLines(file.FullName))
            {
                lineNumber++;
                try
                {
                    var item = JsonConvert.DeserializeObject<JObject>(fileLine);

                    if (item == null)
                    {
                        _logger.LogError("Deserialization of line {LineNumber} of the file '{File}' returns null.", lineNumber, file.FullName);
                        parseErrors.Add($"Deserialization of line {lineNumber} of the file '{file.FullName}' returns null.");
                        continue;
                    }

                    item.Add("sessionId", header.SessionId);
                    item.Add("sourceFilePath", header.SourceFilePath);
                    item["sourceFile"] = header.SourceFile;
                    item.Add("fileTimeStamp", header.FileTimeStamp);
                    item.Add("fileIndex", header.FileIndex);

                    if (dataFilter == null || dataFilter.Match(item))
                    {
                        fileItems.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    parseErrors.Add($"Error parsing or filtering file '{file.FullName}' on line {lineNumber}");
                    _logger.LogError(ex, "Error parsing or filtering file '{File}' on line {Line}", file.FullName, lineNumber);
                }
            }

            return parseErrors;
        }
        /// <summary>
        /// Estimates the query scope.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Instance of the QueryEstimationResult</returns>
        public QueryEstimationResult EstimateQueryScope(SearchLogsRequest request)
        {
            var result = new QueryEstimationResult();

            var validationResult = request.Validate();

            if (!validationResult.IsValid)
            {
                result.ValidationErrors = validationResult;
                return result;
            }

            var parseErrors = new ConcurrentBag<string>();

            var totalCounters = new Counters();

            SystemExtensions.MeasureCodeExecution(() =>
            {
                foreach (var folder in request.Folders)
                {
                    try
                    {
                        var fullPath = Path.GetFullPath(folder);

                        if (!Directory.Exists(fullPath))
                        {
                            parseErrors.Add($"Folder '{fullPath}' configured for reading does not exists.");
                            _logger.LogWarning("Folder '{Folder}' configured for reading does not exists.", fullPath);
                            continue;
                        }

                        var dir = new DirectoryInfo(fullPath);

                        if (string.IsNullOrWhiteSpace(request.FileExtensionFilter))
                        {
                            request.FileExtensionFilter = "*.log";
                            _logger.LogWarning("fileFilter parameter is empty. Fallback to default: '*.log'");
                        }

                        var files = dir.GetFiles(request.FileExtensionFilter);

                        var options = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxDegreeOfParallelism
                        };

                        Parallel.ForEach(files, () => new Counters(), (file, loopState, localCounters) =>
                        {
                            var (fileRecordsCount, estimateErrors) = EstimateFile(file, request.FileDateRangeFilter, request.DataQuery);

                            if (estimateErrors != null && estimateErrors.Any())
                                estimateErrors.ForEach(m => parseErrors.Add(m));

                            localCounters.RecordsCount += fileRecordsCount;

                            if (fileRecordsCount > 0)
                                localCounters.FilesCount++;

                            return localCounters;

                        }, (localCounters) =>
                        {
                            Interlocked.Add(ref totalCounters.RecordsCount, localCounters.RecordsCount);
                            Interlocked.Add(ref totalCounters.FilesCount, localCounters.FilesCount);
                        });

                    }
                    catch (Exception ex)
                    {
                        parseErrors.Add($"Error in processing folder '{folder}'. Error: {ex}");
                        _logger.LogError(ex, "Error in processing folder '{Folder}'", folder);
                    }
                }

                parseErrors.ToList().ForEach(error => result.Errors.Add(error));
                result.FilesCount = totalCounters.FilesCount;
                result.RecordsCount = totalCounters.RecordsCount;

            }, result);

            return result;

        }
        /// <summary>
        /// Calculates the estimation on one file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="dataFilter">The data filter.</param>
        /// <param name="parseErrors">The parse errors.</param>
        /// <returns></returns>
        private (int count, List<string> parseErrors) EstimateFile(FileInfo file, DateRange fileFilter, QueryNode? dataFilter)
        {
            int numRecords = 0;

            var parseErrors = new List<string>();

            var (header, metaErrors) = ParseFileMetaData(file, fileFilter);

            if (metaErrors != null && metaErrors.Any())
            {
                parseErrors.AddRange(metaErrors);
            }

            if (header == null)
                return (0, parseErrors);

            int lineNumber = 0;
            foreach (var fileLine in File.ReadLines(file.FullName))
            {
                lineNumber++;
                try
                {
                    var item = JsonConvert.DeserializeObject<JObject>(fileLine);

                    if (item == null)
                    {
                        _logger.LogError("Deserialization of line {LineNumber} of the file '{File}' returns null.", lineNumber, file.FullName);
                        continue;
                    }

                    item.Add("sessionId", header.SessionId);
                    item.Add("sourceFilePath", header.SourceFilePath);
                    item["sourceFile"] = header.SourceFile;
                    item.Add("fileTimeStamp", header.FileTimeStamp);
                    item.Add("fileIndex", header.FileIndex);

                    if (dataFilter == null || dataFilter.Match(item))
                    {
                        numRecords++;
                    }
                }
                catch (Exception ex)
                {
                    parseErrors.Add($"Error parsing or filtering file '{file.FullName}' on line {lineNumber}");
                    _logger.LogError(ex, "Error parsing or filtering the file '{File}' on line {Line}", file.FullName, lineNumber);
                }
            }

            return (numRecords, parseErrors);
        }
        /// <summary>
        /// Parses the file meta data.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="parseErrors">The parse errors.</param>
        /// <returns>Instance of the AuditLogFileMetaData or null in case file does not match the filter cryteria</returns>
        private (AuditLogFileMetaData? metadata, List<string>? errors) ParseFileMetaData(FileInfo file, DateRange fileFilter)
        {
            var parseErrors = new List<string>();

            string errText = $"Invalid or missing part of the file name {file.FullName}. File name must follow the folowing pattern: SYSTEM-ID_SESSION-ID_yyyyMMddHH_NNN.EXTENSION";

            try
            {
                var fileNameParts = file.Name.Split('_', '.');

                if (fileNameParts.Length < 4 || fileNameParts.Length > 5)
                {
                    parseErrors.Add($"Invalid file name format of the file {file.FullName}");
                    return (null, parseErrors);
                }

                // 2023062814
                if (!DateTime.TryParseExact(fileNameParts[2], "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime timestamp))
                {
                    parseErrors.Add(errText);
                    return (null, parseErrors);
                }

                if (!(DateRange.IsEmpty(fileFilter) || fileFilter.IsInRange(timestamp)))
                    return (null, parseErrors);

                AuditLogFileMetaData item = new AuditLogFileMetaData();
                item.SourceFile = file.Name;
                item.SourceFilePath = Path.GetDirectoryName(file.FullName);
                item.FileTimeStamp = timestamp;
                item.CreatedOn = file.CreationTime;
                item.FileSize = file.Length;

                item.SessionId = fileNameParts[1];
                if (string.IsNullOrWhiteSpace(item.SessionId))
                {
                    parseErrors.Add(errText);
                    return (null, parseErrors);
                }


                if (string.IsNullOrWhiteSpace(fileNameParts[2]))
                {
                    parseErrors.Add(errText);
                    return (null, parseErrors);
                }

                if (fileNameParts.Length == 5)
                {
                    if (string.IsNullOrWhiteSpace(fileNameParts[3]))
                    {
                        parseErrors.Add(errText);
                        return (null, parseErrors);
                    }

                    // 2023062814
                    if (!int.TryParse(fileNameParts[3], out int index))
                    {
                        parseErrors.Add(errText);
                        return (null, parseErrors);
                    }

                    item.FileIndex = index;
                }

                return (item, parseErrors);
            }
            catch (Exception ex)
            {
                parseErrors.Add($"Error parsing the file '{file.FullName}' metadata. Error: {ex}");
            }

            return (null, parseErrors);
        }
        /// <summary>
        /// Filters the files based on a date range filter.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Instance of FileListResult</returns>
        public FileListResult FilterFiles(FilesListRequest request)
        {
            var result = new FileListResult();

            var validationResult = request.Validate();

            if (!validationResult.IsValid)
            {
                result.ValidationErrors = validationResult;
                return result;
            }

            var parseErrors = new ConcurrentBag<string>();
            var filesList = new ConcurrentBag<AuditLogFileMetaData>();

            SystemExtensions.MeasureCodeExecution(() =>
            {
                foreach (var folder in request.Folders)
                {
                    try
                    {
                        var fullPath = Path.GetFullPath(folder);

                        if (!Directory.Exists(fullPath))
                        {
                            parseErrors.Add($"Folder '{fullPath}' configured for reading does not exists.");
                            _logger.LogWarning("Folder '{Folder}' configured for reading does not exists.", fullPath);
                            continue;
                        }

                        var dir = new DirectoryInfo(fullPath);

                        if (string.IsNullOrWhiteSpace(request.FileExtensionFilter))
                        {
                            request.FileExtensionFilter = "*.log";
                            _logger.LogWarning("fileFilter parameter is empty. Fallback to default: '*.log'");
                        }

                        var files = dir.GetFiles(request.FileExtensionFilter, SearchOption.AllDirectories);

                        var options = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxDegreeOfParallelism
                        };

                        Parallel.ForEach(files, options, (file, c) =>
                        {
                            var (item, metaErrors) = ParseFileMetaData(file, request.FileDateRangeFilter);

                            if (metaErrors != null && metaErrors.Any())
                                metaErrors.ForEach(m => parseErrors.Add(m));

                            if (item != null)
                                filesList.Add(item);
                        });

                    }
                    catch (Exception ex)
                    {
                        parseErrors.Add($"Error in processing folder '{folder}'. Error: {ex}");
                        _logger.LogError(ex, "Error in processing folder '{Folder}'", folder);
                    }
                }

                filesList.ToList().ForEach(f => result.Records.Add(f));


            }, result);


            return result;
        }

        /// <summary>
        /// Extracts the content of the file and its metadata.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Instance of FileContentResult</returns>
        public FileContentResult GetFileContent(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                var result = new FileContentResult();
                result.ValidationErrors.Errors.Add(new ValidationFailure
                {
                    AttemptedValue = file,
                    PropertyName = "file",
                    ErrorMessage = "File name is required"
                });

                return result;
            }

            return GetFileContent(new FileInfo(file));
        }

        /// <summary>
        /// Extracts the content of the file and its metadata.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Instance of FileContentResult</returns>
        public FileContentResult GetFileContent(FileInfo file)
        {

            FileContentResult result = new FileContentResult();

            if (file == null)
            {
                result.ValidationErrors.Errors.Add(new ValidationFailure
                {
                    AttemptedValue = file,
                    PropertyName = "file",
                    ErrorMessage = "File name is required"
                });

                return result;
            }

            if (!File.Exists(file.FullName))
            {
                result.ValidationErrors.Errors.Add(new ValidationFailure
                {
                    AttemptedValue = file.FullName,
                    PropertyName = "file",
                    ErrorMessage = $"File '{file.FullName}' does not exists"
                });

                return result;
            }

            ConcurrentBag<string> parseErrors = new ConcurrentBag<string>();

            SystemExtensions.MeasureCodeExecution(() =>
            {

                var (metadata, metaErrors) = ParseFileMetaData(file, DateRange.Empty);

                if (metaErrors != null && metaErrors.Any())
                    metaErrors.ForEach(m => parseErrors.Add(m));

                result.Errors.AddRange(parseErrors);
                result.Metadata = metadata;

                using (var stm = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stm, true))
                    {
                        result.ContentEncoding = reader.CurrentEncoding.BodyName;
                        result.Content = reader.ReadToEnd().TrimEnd().TrimEnd(Environment.NewLine.ToCharArray());
                        result.RecordsCount = result.Content.CountLines();
                    }
                }
            }, result);

            return result;
        }
       
    }
}
