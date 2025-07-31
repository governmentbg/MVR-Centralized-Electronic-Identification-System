using System.Buffers;
using System.Configuration;
using System.Globalization;
using System.Text;
using eID.PJS.AuditLogging;
using eID.PJS.Services.OpenSearch;
using eID.PJS.Services.TimeStamp;
using Newtonsoft.Json;

#nullable disable

namespace eID.PJS.Services.Signing
{

    /// <summary>The service resposible for signing the audit logs</summary>
    public class SigningService : ProcessingServiceBase<SigningServiceState>
    {
        private ILogger<SigningService> _logger;
        private readonly SigningServiceSettings _settings;
        private readonly IFileChecksumAlgorhitm _algorhitm;
        private readonly ITimeStampProvider _timestampProvider;
        private readonly SignServerProviderSettings _ssSettings;
        private readonly IServiceProvider _serviceProvider;

        private const int CHECKSUM_LENGTH = 88;
        private readonly int _maxDegreeOfParallelism;
        private static object _lock = new object();

        /// <summary>Initializes a new instance of the <see cref="SigningService" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="signAlgo">The sign algo.</param>
        /// <param name="timestampProvider">The secure timestamp provider</param>
        /// <exception cref="System.ArgumentNullException">logger
        /// or
        /// settings
        /// or
        /// signAlgo</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// MonitoredFolders parameter is required in the configuration.
        /// or
        /// MonitoredFolders parameters cannot contain empty elements.
        /// </exception>
        public SigningService(ILogger<SigningService> logger, 
                                SigningServiceSettings settings, 
                                IFileChecksumAlgorhitm signAlgo, 
                                SignServerProviderSettings ssSettings,
                                IServiceProvider sp)
        {

            _serviceProvider = sp ?? throw new ArgumentNullException(nameof(sp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _algorhitm = signAlgo ?? throw new ArgumentNullException(nameof(signAlgo));
            _ssSettings = ssSettings ?? throw new ArgumentNullException(nameof(ssSettings));

            if (settings.MonitoredFolders.Count == 0)
                throw new ConfigurationErrorsException("MonitoredFolders parameter is required in the configuration.");

            if (settings.MonitoredFolders.Any(a => string.IsNullOrWhiteSpace(a.MonitorFolder)))
                throw new ConfigurationErrorsException("MonitoredFolders parameters cannot contain empty elements.");

            if (string.IsNullOrWhiteSpace(_settings.StatePath))
            {
                _logger.LogWarning($"StatePath is not defined in configuration. Using default - current directory -> {nameof(SigningService)}.json");
                _settings.StatePath = $"{nameof(SigningService)}.json";
            }

            _maxDegreeOfParallelism = SystemExtensions.SuggestMaxDegreeOfParallelism();
        }

        /// <summary>Start the processing of the audit log files to generate the hash files.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public override SigningServiceState Process()
        {

            lock (_lock)
            {

                var state = new SigningServiceState();

                SystemExtensions.MeasureCodeExecution(() =>
                {
                    var options = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _maxDegreeOfParallelism
                    };

                    Parallel.ForEach(_settings.MonitoredFolders, options, (folder, c) =>
                    {
                        var monitorFolder = new MonitoredFolder
                        {
                            TargetFolderLogs = folder.TargetFolderLogs,
                            TargetFolderHash = folder.TargetFolderHash,
                            MonitorFolder = folder.MonitorFolder,
                        };

                        state.Folders.Add(monitorFolder);

                        ProcessFolder(monitorFolder, _algorhitm);
                    });

                }, state);

                SaveState(state, "signing", _settings);

                return state;
            }

        }

        private void ProcessFolder(MonitoredFolder folder, IFileChecksumAlgorhitm alg)
        {
            _logger.LogDebug("Process Folder {MonitorFolder} entered.", folder.MonitorFolder);
            SystemExtensions.MeasureCodeExecution(() =>
            {

                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism
                };


                if (!Directory.Exists(folder.MonitorFolder))
                {
                    _logger.LogWarning("Monitored folder {MonitorFolder} does not exists", folder.MonitorFolder);
                    return;
                }

                var dir = new DirectoryInfo(folder.MonitorFolder);

                if (string.IsNullOrWhiteSpace(_settings.AuditLogFileExtension))
                {
                    _settings.AuditLogFileExtension = "*.audit";
                    _logger.LogWarning("MonitorFilter parameter is empty. Fallback to default: '*.audit'");
                }

                var targetDirLogs = Path.GetFullPath(folder.TargetFolderLogs);
                var targetDirHash = Path.GetFullPath(folder.TargetFolderHash);

                if (!Directory.Exists(targetDirLogs))
                    Directory.CreateDirectory(targetDirLogs);

                if (!Directory.Exists(targetDirHash))
                    Directory.CreateDirectory(targetDirHash);


                _logger.LogDebug("Start parallel foreach for {Directory}.", dir.FullName);
                Parallel.ForEach(dir.EnumerateFiles($"*{_settings.AuditLogFileExtension}"), options, (fileItem, c) =>
                {
                    var file = new MonitoredFile
                    {
                        Name = fileItem.Name,
                        Size = fileItem.Length,
                        Folder = Path.GetDirectoryName(fileItem.FullName),
                        Status = MonitoredResourceStatus.Ready,
                    };

                    folder.Files.Add(file);

                    try
                    {
                        SystemExtensions.MeasureCodeExecution(() =>
                        {

                            if (!File.Exists(file.FullName))
                            {
                                file.Status = MonitoredResourceStatus.Error;
                                file.Error = new Exception($"File '{file.FullName}' does not exists.");
                                _logger.LogWarning("File '{FullName}' does not exists.", file.FullName);
                                return;
                            }



                            var checksum = CalculateChecksum(file, alg);

                            string checksumFileFullName = Path.Combine(file.Folder, Path.GetFileNameWithoutExtension(file.Name) + _settings.HashFileExtension);

                            // Try to skip the existing hash files if they have the same checksum as the calculated one, otherwise raise an error.
                            if (File.Exists(checksumFileFullName))
                            {
                                _logger.LogDebug("Checksum file exists {ChecksumFile}.", checksumFileFullName);
                                var checksumFileContent = File.ReadAllText(checksumFileFullName);
                                var checksumSource = JsonConvert.DeserializeObject<AuditChecksumRecord>(checksumFileContent);

                                if (checksumSource != null && checksumSource.Checksum == checksum?.Checksum)
                                {
                                    file.Status = MonitoredResourceStatus.Skipped;
                                    _logger.LogInformation("Found existing valid checksum file '{ChecksumFile}' for '{LogFile}'. Skipping!", checksumFileFullName, file.Name);
                                }
                                else
                                {
                                    string msg = $"The calculated checksum of the log file {file.FullName} differs from the checksum in the checksum file '{checksumFileFullName}'.";
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = new Exception(msg);
                                    _logger.LogError(msg);
                                }
                            }
                            else // Generate new hash file
                            {
                                _logger.LogDebug("Checksum file {ChecksumFile} will be generated.", checksumFileFullName);
                                if (string.IsNullOrEmpty(checksum?.Checksum))
                                {
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = new Exception($"Checksum is empty for file: {file.FullName}");
                                    _logger.LogError($"Checksum is empty for file: {file.FullName}");
                                    return;
                                }

                                CreateHashFile(file, checksum, _settings.HashFileExtension);
                            }

                            if (File.Exists(checksumFileFullName))
                            {
                                _logger.LogDebug("Checksum file {ChecksumFile} will be generated.", checksumFileFullName);
                                (var newTargetDirLogs, var errLogs) = EnsureFolderTreeExists(targetDirLogs, file.Name);
                                (var newTargetDirHash, var errHash) = EnsureFolderTreeExists(targetDirHash, file.Name);

                                if (errLogs != null)
                                {
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = errLogs;
                                    _logger.LogError(errLogs, "Error creating folder structure for audit log files based on the audit log file name {name}", file.Name);
                                    return;
                                }

                                if (errHash != null)
                                {
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = errHash;
                                    _logger.LogError(errHash, "Error creating folder structure for hash files based on the audit log file name {name}", file.Name);
                                    return;
                                }

                                //Move audit log file and hash file to the respective signed folders
                                var targedAuditLogFile = Path.Combine(newTargetDirLogs, Path.GetFileNameWithoutExtension(file.Name) + _settings.AuditLogFileExtension);
                                var targedChecksumFile = Path.Combine(newTargetDirHash, Path.GetFileNameWithoutExtension(file.Name) + _settings.HashFileExtension);

                                _logger.LogDebug("Moving Checksum file {ChecksumFile} to {TargetChecksumFile}.", checksumFileFullName, targedChecksumFile);
                                File.Move(checksumFileFullName, targedChecksumFile);
                                _logger.LogDebug("Moving file {ChecksumFile} to {TargetFile}.", fileItem.FullName, targedAuditLogFile);
                                File.Move(fileItem.FullName, targedAuditLogFile);

                                if (!File.Exists(targedChecksumFile))
                                {
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = new Exception($"Created checksum file not found in the new location '{targedChecksumFile}'!");
                                    _logger.LogError("Created checksum file not found in the new location '{targedFile}'!", targedChecksumFile);
                                    return;
                                }

                                if (!File.Exists(targedAuditLogFile))
                                {
                                    file.Status = MonitoredResourceStatus.Error;
                                    file.Error = new Exception($"Audit log file not found in the new location '{targedAuditLogFile}'!");
                                    _logger.LogError("Audit log file not found in the new location '{targedFile}'!", targedAuditLogFile);
                                    return;
                                }

                                file.Status = MonitoredResourceStatus.Processed;
                                _logger.LogDebug("Created a new checksum file '{FullName}'", file.FullName);
                            }
                            else
                            {
                                file.Status = MonitoredResourceStatus.Error;
                                file.Error = new Exception($"Created new checksum file '{file.FullName}' not found after saving!");
                                _logger.LogError("Created new checksum file '{FullName}' not found after saving!", file.FullName);
                            }

                        }, file);
                    }
                    catch (Exception ex)
                    {
                        file.Status = MonitoredResourceStatus.Error;
                        file.Error = ex;
                        _logger.LogError(ex, "Error trying to generate checksum for the file '{file}'", file.FullName);
                    }

                });

            }, folder);
        }
        private void CreateHashFile(MonitoredFile file, ChecksumData data, string hashFileExtension)
        {
            var provider = _serviceProvider.GetRequiredService<ITimeStampProvider>();

            var timestamp = provider.RequestToken(data.Checksum).GetAwaiter().GetResult();

            if (timestamp.Error != null)
            {
                throw timestamp.Error;
            }

            var record = new AuditChecksumRecord
            {
                Checksum = data.Checksum,
                DateCreated = timestamp.TimestampToken.TokenInfo.Timestamp.UtcDateTime,
                SourceFile = file.Name,
                SourcePath = file.Folder,
                Events = data.EventIds.ToDelimitedText(","),
                SystemId = data.SystemId,
                Timestamp = timestamp.Response
            };


            var fileContent = JsonConvert.SerializeObject(record, Formatting.None) + Environment.NewLine; //Append new line for Vector to parse it!

            var filePath = Path.Combine(Path.GetFullPath(file.Folder), Path.GetFileNameWithoutExtension(file.Name) + hashFileExtension);

            _logger.LogDebug("Storing all data to file {FilePath}.", filePath);
            File.WriteAllText(filePath, fileContent);
        }
        private ChecksumData CalculateChecksum(MonitoredFile file, IFileChecksumAlgorhitm alg)
        {

            var fileContent = File.ReadAllLines(file.FullName, Encoding.UTF8);

            if (fileContent == null || fileContent.Length == 0)
            {
                file.Status = MonitoredResourceStatus.Error;
                file.Error = new Exception($"Log file: '{file.FullName}' is empty.");
                _logger.LogWarning($"Log file: '{file.FullName}' is empty.");
                return null;
            }

            var events = new SortedList<string, AuditLogRecord>();
            int bufferLength = 0;
            int lineIndex = 0;

            foreach (var line in fileContent)
            {
                lineIndex++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogWarning($"Empty line found in file '{file.Folder}'");
                    continue;
                }

                var e = JsonConvert.DeserializeObject<AuditLogRecord>(line);

                if (e == null)
                {
                    // Skip the problematic line
                    var msg = $"File '{file.FullName}' contains line that cannot be deserialized. Line ({lineIndex}): <line>{line}</line>";
                    file.Status = MonitoredResourceStatus.Error;
                    file.Error = new Exception(msg);

                    _logger.LogError(msg);

                    continue;
                }

                // Exclude FileSource from the calculation of the checksum because it does not exists when the log is created and is not signifficant information.
                // Current implementation does not use the whole event object but only the checksum field. To prevent problems in future implementations
                e.SourceFile = null;
                events.Add(e.EventId, e);
                bufferLength += CHECKSUM_LENGTH;
            }

            return new ChecksumData
            {
                Checksum = alg.Calculate(events.Values, bufferLength),
                EventIds = events.Select(s => s.Value.EventId).ToArray(),
                SystemId = events.First().Value.SystemId
            };

        }

        private (string Path, Exception Error) EnsureFolderTreeExists(string targetDir, string auditLogFileName)
        {
            var result = ExtractFileTimeStamp(auditLogFileName);

            if (result.Error != null)
                return (null, result.Error);

            string dir = Path.Combine(targetDir, result.TimeStamp.Value.Year.ToString("D4"), result.TimeStamp.Value.Month.ToString("D2"), result.TimeStamp.Value.Day.ToString("D2"));

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return (dir, null);
        }

        private (DateTime? TimeStamp, Exception Error) ExtractFileTimeStamp(string auditLogFileName)
        {
            string errText = $"Invalid or missing part of the file name {auditLogFileName}. File name must follow the folowing pattern: SYSTEM-ID_SESSION-ID_yyyyMMddHH_NNN.EXTENSION";

            try
            {
                var fileNameParts = auditLogFileName.Split('_', '.');

                if (fileNameParts.Length < 4 || fileNameParts.Length > 5)
                {
                    return (null, new Exception(errText));
                }


                // 2023062814
                if (!DateTime.TryParseExact(fileNameParts[2], "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime timestamp))
                {
                    return (null, new Exception(errText));
                }

                return (timestamp, null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }


    }
}
