using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using eID.PJS.AuditLogging;
using eID.PJS.Services.Entities;
using eID.PJS.Services.OpenSearch;
using eID.PJS.Services.TimeStamp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static eID.PJS.Services.CollectionExtensions;

namespace eID.PJS.Services.Verification;

/// <summary>Audit Log Verification Service. It executes the process of the audit log verificaition</summary>
public class VerificationService : ProcessingServiceBase<VerificationServiceStateShort>
{
    private readonly ILogger<VerificationService> _logger;
    private readonly VerificationServiceSettings _settings;
    private readonly OpenSearchManager _osManager;
    private readonly IFileChecksumAlgorhitm _fileChecksumAlg;
    private readonly ICryptoKeyProvider _cryptoProvider;
    private ICommandStateProvider<VerificationServiceStateShort> _commandStateProvider;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly int _maxDegreeOfParallelism;
    private const int FALSE_NEGATIVE_RISK_PERIOD_MINUTES = 5;
    /// <summary>Initializes a new instance of the <see cref="VerificationService" /> class.</summary>
    /// <param name="logger">The logger.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="mgr">The MGR.</param>
    /// <param name="fileChecksumAlg">The file checksum alg.</param>
    /// <param name="cryptoProvider">The crypto provider.</param>
    /// <param name="commandStateProvider">The command state provider.</param>
    /// <param name="scopeFactory"></param>
    /// <exception cref="System.ArgumentNullException">logger
    /// or
    /// settings
    /// or
    /// mgr
    /// or
    /// fileChecksumAlg
    /// or
    /// cryptoProvider
    /// or
    /// commandStateProvider</exception>
    public VerificationService(ILogger<VerificationService> logger,
                                VerificationServiceSettings settings,
                                OpenSearchManager mgr,
                                IFileChecksumAlgorhitm fileChecksumAlg,
                                ICryptoKeyProvider cryptoProvider,
                                ICommandStateProvider<VerificationServiceStateShort> commandStateProvider,
                                IServiceScopeFactory scopeFactory

        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _osManager = mgr ?? throw new ArgumentNullException(nameof(mgr));
        _fileChecksumAlg = fileChecksumAlg ?? throw new ArgumentNullException(nameof(fileChecksumAlg));
        _cryptoProvider = cryptoProvider ?? throw new ArgumentNullException(nameof(cryptoProvider));
        _commandStateProvider = commandStateProvider ?? throw new ArgumentNullException(nameof(commandStateProvider));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory)); ;

        _maxDegreeOfParallelism = SystemExtensions.SuggestMaxDegreeOfParallelism();
    }

    /// <summary>
    /// Method to start the verification process from the scheduler.
    /// It will scan for all configured System Ids for the period configured in the configuration file
    /// </summary>
    /// <returns>Instance of VerificationServiceState </returns>
    public override VerificationServiceStateShort? Process()
    {
        string commandType = typeof(VerifyPeriodCommand).FullName ?? typeof(VerifyPeriodCommand).Name;


        if (_settings.VerifyPeriod == VerificationCheckPeriod.Disabled)
        {
            _logger.LogWarning("Verification check period is set to disabled. Skipping verification.");
            return null;
        }

        DateTime startDate = DateTime.Now;
        DateTime endDate = _settings.UseLocalTime ? DateTime.Now : DateTime.UtcNow;

        try
        {
            switch (_settings.VerifyPeriod)
            {
                case VerificationCheckPeriod.All:
                    commandType = typeof(VerifyAllCommand).FullName ?? typeof(VerifyPeriodCommand).Name;
                    break;

                case VerificationCheckPeriod.Day:
                    startDate = endDate.AddDays(-1);
                    endDate = endDate.Add(-_settings.VerifyPeriodOffset);
                    break;

                case VerificationCheckPeriod.Week:
                    startDate = endDate.AddDays(-7);
                    break;

                case VerificationCheckPeriod.TwoWeeks:
                    startDate = endDate.AddDays(-14);
                    break;

                case VerificationCheckPeriod.Month:
                    startDate = endDate.AddMonths(-1);
                    break;

                case VerificationCheckPeriod.Quarter:
                    startDate = endDate.AddMonths(-3);
                    break;

                case VerificationCheckPeriod.Year:
                    startDate = endDate.AddYears(-1);
                    break;

                default:
                    throw new ArgumentException("Unknown VerifyPeriod period specified");
            }

            VerificationServiceStateShort result;

            if (_commandStateProvider.IsCommandInProgress(commandType))
            {
                _logger.LogWarning("Another task of type {commandType} is already in progress. Skipping this schedule run.", commandType);

                return new VerificationServiceStateShort
                {
                    Status = VerificationStatus.Canceled,
                    Message = $"Another task of type {commandType} is already in progress. Skipping this schedule run.",
                };
            }

            var taskId = Guid.NewGuid();
            _commandStateProvider.SetCommandInProgress(commandType, taskId);
            _logger.LogDebug("SetCommandInProgress '{cmd}'", commandType);

            if (_settings.VerifyPeriod == VerificationCheckPeriod.All)
            {
                result = VerifyAll();
            }
            else
            {
                result = VerifyPeriod(startDate, endDate.Add(-_settings.VerifyPeriodOffset));
            }

            _commandStateProvider.StoreResult(taskId, result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scheduled virification for period {period}", _settings.VerifyPeriod);
        }
        finally
        {
            _commandStateProvider.RemoveCommandInProgress(commandType);
            _logger.LogDebug("RemoveCommandInProgress '{cmd}'", commandType);
        }

        return null;
    }

    /// <summary>
    /// Verifies all audit logs for the configured System IDs
    /// </summary>
    /// <returns>Instance of VerificationServiceState </returns>
    public VerificationServiceStateShort VerifyAll()
    {

        _logger.LogInformation("Verifying all logs...");

        var state = new VerificationServiceState();

        try
        {
            SystemExtensions.MeasureCodeExecution(() =>
            {
                //Check if OpenSearch is available, otherwise cancel verification.
                var canContinue = _osManager.Ping();

                if (canContinue)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var exclusionsProvider = scope.ServiceProvider.GetRequiredService<IVerificationExclusionProvider>();

                        var exclusions = new Exclusions(exclusionsProvider);

                        exclusions.Reload();

                        var options = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxDegreeOfParallelism
                        };

                        foreach (var index in _settings.Systems)
                        {
                            try
                            {
                                var systemPaths = _settings.Systems[index.Key];
                                var dateOfOldestAvailableRecord = IOExtensions.GetOldestNonEmptyCalendarFolderDateInPath(systemPaths.AuditLogs);
                                var datesWithoutLogs = IOExtensions.GetDatesFromPeriodWithMissingOrEmptyCalendarFolders(systemPaths.AuditLogs, dateOfOldestAvailableRecord, DateTime.MaxValue);

                                var systemResult = new SystemVerificationResult()
                                {
                                    IndexName = _osManager.GetOpenSearchIndexName(index.Key).LogsIndex,
                                    LocalLogsLocation = index.Value.AuditLogs,
                                    SystemId = index.Key,
                                    DatesWithoutLogs = datesWithoutLogs,
                                    DateOfOldestAvailableRecord = dateOfOldestAvailableRecord
                                };

                                state.Systems.Add(systemResult);

                                int batchSize = 50 * _maxDegreeOfParallelism;

                                var pathToScan = Path.GetFullPath(index.Value.AuditLogs);

                                // Check if the configured folder exists.
                                // If not break the whole operation because:
                                // - There can be an error in the configuration.
                                // - There can be a problem with the storage.
                                // - Someone has deleted the folder and those makes the logs unverifeable.
                                if (!Directory.Exists(pathToScan))
                                {
                                    systemResult.Errors.Add($"Folder for the index '{systemResult.IndexName}', '{pathToScan}' does not exists.");
                                    systemResult.Status = VerificationStatus.ErrorAborted;
                                    state.Status = VerificationStatus.ErrorAborted;
                                    _logger.LogError("Folder for the index '{IndexName}', '{pathToScan}' does not exists.", systemResult.IndexName, pathToScan);

                                    break;
                                }

                                foreach (var files in IOExtensions.EnumerateFilesInBatches(pathToScan, _settings.AuditLogFileExtension, batchSize))
                                {

                                    if (files != null && files.Any())
                                    {
                                        Parallel.ForEach(files, options, (file, c) =>
                                        {

                                            var tsProvider = scope.ServiceProvider.GetRequiredService<ITimeStampProvider>();
                                            var tsCertChain = scope.ServiceProvider.GetRequiredService<X509Certificate2>();

                                            var fileResult = VerifyFile(file, exclusions, tsProvider, tsCertChain);

                                            switch (fileResult.Status)
                                            {
                                                case VerificationStatus.ErrorAborted:
                                                    systemResult.Status = VerificationStatus.ErrorAborted;
                                                    systemResult.Files.Add(fileResult);
                                                    c.Break();
                                                    break;
                                                case VerificationStatus.CompletedWithErrors:
                                                    systemResult.Files.Add(fileResult);
                                                    systemResult.Status = VerificationStatus.CompletedWithErrors;
                                                    break;
                                            }

                                        });

                                        if (systemResult.Status == VerificationStatus.ErrorAborted)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        systemResult.Status = VerificationStatus.CompletedWithErrors;
                                        systemResult.Errors.Add($"Index '{systemResult.IndexName}' with local log files in '{index.Value}' has no files found to verify.");
                                    }


                                    if (systemResult.Status == VerificationStatus.ErrorAborted)
                                    {
                                        state.Status = VerificationStatus.ErrorAborted;

                                        if (systemResult.Errors != null && systemResult.Errors.Any())
                                        {
                                            _logger.LogError("Verification process aborted in system/index '{index}', local logs location '{logs}'. See errors below:", systemResult.IndexName, systemResult.LocalLogsLocation);

                                            foreach (var err in systemResult.Errors)
                                            {
                                                _logger.LogError(err);
                                            }
                                        }

                                        break;
                                    }

                                    if (systemResult.Status == VerificationStatus.CompletedWithErrors)
                                    {
                                        state.Status = VerificationStatus.CompletedWithErrors;
                                    }
                                }

                                if (systemResult.Status == VerificationStatus.NotExecuted)
                                    systemResult.Status = VerificationStatus.Finished;
                            }
                            catch (Exception ex)
                            {
                                state.Errors.Add(ex.ToString());
                                state.Status = VerificationStatus.ErrorAborted;
                                _logger.LogError(ex, "Error verifying systemId '{index}'", index.Key);
                            }

                            if (state.Status == VerificationStatus.ErrorAborted)
                            {
                                break;
                            }


                        } //foreach (var index in _settings.Systems)

                    } //using (var scope = _scopeFactory.CreateScope())
                }
                else
                {
                    state.Errors.Add("Verification canceled due to OpenSearch not responding.");
                    _logger.LogError("Verification canceled due to OpenSearch not responding.");
                    state.Status = VerificationStatus.Canceled;
                }
            }, state);

            if (state.Status == VerificationStatus.NotExecuted)
                state.Status = VerificationStatus.Finished;

            SaveVerificationStates(state, _settings);

            _logger.LogInformation("Verifying all logs completed.");
        }
        catch (Exception ex)
        {
            state.Status = VerificationStatus.ErrorAborted;
            state.Errors.Add(ex.ToString());

            _logger.LogError("Error executing VerifyAll. {ex}", ex);
        }

        return state.ToShort();
    }


    /// <summary>
    /// Verifies all audit logs for the specified period for all configured System IDs.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>Instance of VerificationServiceState </returns>
    public VerificationServiceStateShort VerifyPeriod(DateTime? startDate, DateTime? endDate)
    {
        return VerifyPeriod(null, startDate, endDate);
    }

    /// <summary>
    /// Verifies all audit logs for the specified period and System ID.
    /// </summary>
    /// <param name="systemId">The system identifier.</param>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <returns>Instance of VerificationServiceState </returns>
    /// <exception cref="System.ArgumentException">
    /// Both FromDate and ToDate cannot be null - fromDate
    /// or
    /// FromDate must be before ToDate - fromDate
    /// </exception>
    public VerificationServiceStateShort VerifyPeriod(string? systemId, DateTime? fromDate, DateTime? toDate)
    {
        var state = new VerificationServiceState();

        if (fromDate == null && toDate == null)
        {
            state.Status = VerificationStatus.ErrorAborted;
            state.Errors.Add("Both FromDate and ToDate cannot be null");
            _logger.LogError("Both FromDate and ToDate cannot be null");

            return state.ToShort();
        }

        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Kind != toDate.Value.Kind)
        {
            state.Status = VerificationStatus.ErrorAborted;
            state.Errors.Add("fromDate and toDate must have the same kind.");
            _logger.LogError("fromDate and toDate must have the same kind.");

            return state.ToShort();
        }

        if (fromDate >= toDate)
        {
            state.Status = VerificationStatus.ErrorAborted;
            state.Errors.Add("fromDate must be before toDate");
            _logger.LogError("fromDate must be before toDate");

            return state.ToShort();
        }

        var startDate = fromDate ?? DateTime.MinValue;
        var endDate = toDate ?? DateTime.MaxValue;

        _logger.LogInformation("Verifying logs for period '{FromDate} - {ToDate}'{utc} and system ID {systemId}", startDate, endDate, _settings.UseLocalTime ? "" : " (in UTC)", string.IsNullOrWhiteSpace(systemId) ? "ALL" : systemId);

        try
        {
            SystemExtensions.MeasureCodeExecution(() =>
            {

                using (var scope = _scopeFactory.CreateScope())
                {
                    var exclusionsProvider = scope.ServiceProvider.GetRequiredService<IVerificationExclusionProvider>();

                    var exclusions = new Exclusions(exclusionsProvider);

                    exclusions.Reload();

                    if (DateTime.Now.AddMinutes(-FALSE_NEGATIVE_RISK_PERIOD_MINUTES) <= endDate &&
                       endDate <= DateTime.Now.AddMinutes(FALSE_NEGATIVE_RISK_PERIOD_MINUTES))
                    {
                        state.Message = $"WARNING: The end date of the period you specified ('{startDate}' - '{endDate}') is too close to the current moment and there is a risk to verify log files which are not yet redy for verifying! This can lead to false negative results.";
                        _logger.LogWarning(state.Message);
                    }

                    if (exclusions.IsExcluded(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate)))
                    {
                        state.Status = VerificationStatus.Excluded;
                        state.Message = $"Verification stopped because the provided period from {startDate} to {endDate} falls under configured exclusion and is excluded entirely.";
                        return;
                    }

                    var canContinue = _osManager.Ping();

                    if (canContinue)
                    {
                        var options = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxDegreeOfParallelism
                        };

                        //It can be done in parallel but sounds too aggressive...
                        //Parallel.ForEach(_settings.IndexFolders, (item) => { });

                        foreach (var item in _settings.Systems)
                        {
                            if (!string.IsNullOrWhiteSpace(systemId) && !item.Key.Equals(systemId, StringComparison.OrdinalIgnoreCase))
                                continue;

                            try
                            {
                                var index = _osManager.GetOpenSearchIndexName(item.Key);
                                var systemPaths = _settings.Systems[item.Key];
                                var dateOfOldestAvailableRecord = IOExtensions.GetOldestNonEmptyCalendarFolderDateInPath(systemPaths.AuditLogs);
                                // We start checking for missing files from whatever comes second the requested date, or the oldest available
                                // as by definition if the requested period precedes the available ones there will be no records for that area
                                var mostRecentAvailableStartDate = new DateTime[] { startDate, dateOfOldestAvailableRecord }.Max();
                                var datesWithNoLogs = IOExtensions.GetDatesFromPeriodWithMissingOrEmptyCalendarFolders(systemPaths.AuditLogs, mostRecentAvailableStartDate, toDate ?? DateTime.MaxValue);

                                var systemResult = new SystemVerificationResult()
                                {
                                    IndexName = index.LogsIndex,
                                    LocalLogsLocation = systemPaths.AuditLogs,
                                    SystemId = item.Key,
                                    DatesWithoutLogs = datesWithNoLogs,
                                    DateOfOldestAvailableRecord = dateOfOldestAvailableRecord
                                };

                                state.Systems.Add(systemResult);

                                int batchSize = 100 * _maxDegreeOfParallelism; // Well this can be fine-tuned at some point.

                                var pathToScan = Path.GetFullPath(systemPaths.AuditLogs);

                                // Check if the configured folder exists.
                                // If not break the whole operation because:
                                // - There can be an error in the configuration.
                                // - There can be a problem with the storage.
                                // - Someone has deleted the folder and those makes the logs unverifiable.
                                if (!Directory.Exists(pathToScan))
                                {
                                    systemResult.Errors.Add($"Folder for the index '{index.LogsIndex}', '{pathToScan}' does not exists.");
                                    systemResult.Status = VerificationStatus.ErrorAborted;
                                    state.Status = VerificationStatus.ErrorAborted;
                                    break;
                                }

                                foreach (var files in IOExtensions.EnumerateFilesInBatches(pathToScan, _settings.AuditLogFileExtension, batchSize, f =>
                                {

                                    var parseResult = ParseFileMetaData(f);

                                    if (parseResult.Metadata == null || parseResult.Metadata.FileTimeStamp == DateTime.MinValue)
                                        return false;

                                    if (_settings.UseLocalTime || (startDate.Kind != DateTimeKind.Utc && endDate.Kind != DateTimeKind.Utc))
                                    {
                                        if (parseResult.Metadata.FileTimeStamp >= startDate && parseResult.Metadata.FileTimeStamp <= endDate)
                                            return true;
                                    }
                                    else
                                    {
                                        if (parseResult.Metadata.FileTimeStamp.ToUniversalTime() >= startDate && parseResult.Metadata.FileTimeStamp.ToUniversalTime() <= endDate)
                                            return true;
                                    }

                                    return false;
                                }))
                                {

                                    if (files != null && files.Any())
                                    {
                                        Parallel.ForEach(files, options, (file, c) =>
                                        {
                                            var tsProvider = scope.ServiceProvider.GetRequiredService<ITimeStampProvider>();
                                            var tsCertChain = scope.ServiceProvider.GetRequiredService<X509Certificate2>();

                                            var fileResult = VerifyFile(file, exclusions, tsProvider, tsCertChain);

                                            switch (fileResult.Status)
                                            {
                                                case VerificationStatus.ErrorAborted:
                                                    systemResult.Status = VerificationStatus.ErrorAborted;
                                                    systemResult.Files.Add(fileResult);
                                                    c.Break();
                                                    break;
                                                case VerificationStatus.CompletedWithErrors:
                                                    systemResult.Status = VerificationStatus.CompletedWithErrors;
                                                    systemResult.Files.Add(fileResult);
                                                    break;
                                            }
                                        });

                                        if (systemResult.Status == VerificationStatus.ErrorAborted)
                                            break;
                                    }
                                    else
                                    {
                                        systemResult.Status = VerificationStatus.CompletedWithErrors;
                                        systemResult.Errors.Add($"Index '{index.LogsIndex}' with local log files in '{systemPaths}' has no files found to verify.");
                                    }
                                }

                                // Buble up the state in all levels of hierarchy
                                if (systemResult.Status == VerificationStatus.ErrorAborted)
                                {
                                    state.Status = VerificationStatus.ErrorAborted;

                                    if (systemResult.Errors != null && systemResult.Errors.Any())
                                    {
                                        _logger.LogError("Verification process aborted in system/index '{index}', local logs location '{logs}'. See errors below:", systemResult.IndexName, systemResult.LocalLogsLocation);

                                        foreach (var err in systemResult.Errors)
                                        {
                                            _logger.LogError(err);
                                        }
                                    }

                                    break;
                                }

                                if (systemResult.Status == VerificationStatus.CompletedWithErrors)
                                {
                                    state.Status = VerificationStatus.CompletedWithErrors;
                                }

                                if (systemResult.Status == VerificationStatus.NotExecuted)
                                    systemResult.Status = VerificationStatus.Finished;
                            }
                            catch (Exception ex)
                            {
                                state.Status = VerificationStatus.ErrorAborted;
                                state.Errors.Add(ex.ToString());

                                _logger.LogError(ex, "Error verifying systemId '{systemId}'", systemId);
                            }

                            if (state.Status == VerificationStatus.ErrorAborted)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        state.Status = VerificationStatus.Canceled;
                        state.Errors.Add("Verification canceled due to OpenSearch not responding.");
                    }
                }
            }, state);

            if (state.Status == VerificationStatus.NotExecuted)
                state.Status = VerificationStatus.Finished;

            _logger.LogInformation("Verifying audit logs for period '{startDate} - {endDate}'{utc} completed.", startDate, endDate, _settings.UseLocalTime ? "" : " (in UTC)");

            SaveVerificationStates(state, _settings);
        }
        catch (Exception ex)
        {
            state.Status = VerificationStatus.ErrorAborted;
            state.Errors.Add(ex.ToString());

            _logger.LogError(ex, "Error verifying audit logs for period '{startDate} - {endDate}'.", startDate, endDate);
        }

        return state.ToShort();
    }

    /// <summary>
    /// Verifies the file.
    /// </summary>
    /// <param name="fileFullPath">The file full path.</param>
    /// <param name="exclusions"></param>
    /// <param name="tsProvider"></param>
    /// <returns>FileVerificationResult containing the detailed results of the verification process</returns>
    public FileVerificationResult VerifyFile(string fileFullPath, Exclusions exclusions, ITimeStampProvider tsProvider, X509Certificate2 tsCertChain)
    {
        string localLogFileName = Path.GetFileName(fileFullPath);

        var result = new FileVerificationResult()
        {
            SourceFile = localLogFileName,
            Status = VerificationStatus.Finished,
        };

        try
        {

            if (string.IsNullOrWhiteSpace(localLogFileName))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add("File name must not be empty.");
                return result;
            }

            if (exclusions.IsExcluded(Path.GetFullPath(fileFullPath)))
            {
                result.Status = VerificationStatus.Excluded;
                return result;
            }

            var parseResult = ParseFileMetaData(localLogFileName);

            if (parseResult.Errors != null && parseResult.Errors.Any())
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors = parseResult.Errors;
                return result;
            }

            if (parseResult.Metadata == null || string.IsNullOrWhiteSpace(parseResult.Metadata.SystemId) || !_settings.Systems.ContainsKey(parseResult.Metadata.SystemId))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Error trying to get configuration for file '{localLogFileName}'. Either invalid file metadata, missing or invalid SystemID or missing configuration for the particular SystemID");
                return result;
            }

            if (exclusions.IsExcluded(DateOnly.FromDateTime(parseResult.Metadata.FileTimeStamp)))
            {
                result.Status = VerificationStatus.Excluded;
                return result;
            }

            // Get audit log and hash OpenSearch index names.
            string logsIndexName = _osManager.GetOpenSearchIndexName(parseResult.Metadata?.SystemId).LogsIndex;
            string hashIndexName = _osManager.GetOpenSearchIndexName(parseResult.Metadata?.SystemId).HashIndex;

            // Do we have configuration specified for the particular System ID
            if (!_settings.Systems.ContainsKey(parseResult.Metadata!.SystemId))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not find location of local logs specified for systemId {parseResult.Metadata?.SystemId}");
                return result;
            }

            var locationRootStringLogs = _settings.Systems[parseResult.Metadata!.SystemId].AuditLogs;
            var locationRootStringHashes = _settings.Systems[parseResult.Metadata!.SystemId].Hashes;


            // Check if the configured folder to search for logs actually exists.
            if (!Directory.Exists(locationRootStringLogs))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not find folder '{locationRootStringLogs}' for the local logs specified for system {logsIndexName}");
                return result;
            }

            // Check if the configured folder to search for hashesh actually exists.
            if (!Directory.Exists(locationRootStringHashes))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not find folder '{locationRootStringHashes}' for the local hashes specified for system {logsIndexName}");
                return result;
            }



            // Try to find the local log file and hash file based on the calendar notation.
            var localLogFile = fileFullPath;

            var localHashFile = Path.Combine(locationRootStringHashes,
                                      parseResult.Metadata.FileTimeStamp.Year.ToString("D4"),
                                      parseResult.Metadata.FileTimeStamp.Month.ToString("D2"),
                                      parseResult.Metadata.FileTimeStamp.Day.ToString("D2"),
                                      Path.GetFileNameWithoutExtension(localLogFileName) + _settings.HashFileExtension.Trim('*')
                                      );


            if (!File.Exists(localLogFile))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not locate the local log file '{localLogFileName}' under the path '{locationRootStringLogs}'");
                return result;
            }

            if (!File.Exists(localHashFile))
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not locate the local hash file '{localHashFile}' under the path '{locationRootStringLogs}'");
                return result;
            }

            // Load all log records from the local audit log file
            var sortedLocalLogRecords = LoadRecordsFromFile(localLogFile);

            if (sortedLocalLogRecords == null || !sortedLocalLogRecords.Any())
            {
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Local log file '{localLogFile}' is empty. Found under the path '{locationRootStringLogs}'");
                return result;
            }


            // Verify logs from file by recalculating record level hashes
            VerifyEachRecordForFile(result, sortedLocalLogRecords, localLogFile);


            // Get all audit log records from OpenSearch that belong to the specified audit log file.
            // If for some reason it cannot get them, then add the error and skip this log file verification.
            var sortedOsRecordsResult = _osManager.GetAuditLogsForFile(logsIndexName, localLogFileName);

            if (sortedOsRecordsResult.IsValid)
            {
                var sortedOsRecords = sortedOsRecordsResult.Data.ToSortedList(k => k.EventId);
                if (sortedOsRecords == null || !sortedOsRecords.Any())
                {
                    result.Status = VerificationStatus.CompletedWithErrors;
                    result.Errors.Add($"No records found in OpenSearch matching the file name '{localLogFileName}'");
                    return result;
                }

                // Compare the two lists for differences. For the source of truth is used the records from the OpenSearch.
                var difference = sortedOsRecords.CompareTo(sortedLocalLogRecords, c => c.EventId);

                // Compare the OpenSearch records to LocalLogs records by EventId to see if all events are there and there are no missing or added events.
                if (difference.Any())
                {
                    result.Errors.Add($"The records in OpenSearch for the file '{localLogFileName}' differ from the local logs file. Local Logs: {sortedLocalLogRecords.Count}, OpenSearch: {sortedOsRecords.Count()}. See next errors for list of differences.");
                    result.Status = VerificationStatus.CompletedWithErrors;
                    foreach (var item in difference)
                    {
                        string location = item.Location == ListLocation.Left ? "Not found or different in LocalLogs" : "Not found or different in OpenSearch";
                        result.Errors.Add($"{item.Value.EventId}, {location}");
                    }
                }


                // Calculate again the checksum for the whole file, for the records we retrieve from the OpenSearch for the particular local log file
                var osCalculatedRecordsChecksum = _fileChecksumAlg.Calculate(sortedOsRecords.Values);
                _logger.LogDebug("osCalculatedRecordsChecksum: {Stringified}", JsonConvert.SerializeObject(sortedOsRecords.Values));
                // Calculate again the checksum for the whole file for the records we retrieve from the local log file
                var localLogRecordsChecksum = _fileChecksumAlg.Calculate(sortedLocalLogRecords.Values);
                _logger.LogDebug("localLogRecordsChecksum: {Stringified}", JsonConvert.SerializeObject(sortedLocalLogRecords.Values));

                var osChecksumRecordResult = _osManager.GetHashForFile(hashIndexName, localLogFileName);

                if (osChecksumRecordResult.IsValid)
                {

                    // Read the data for the hash file stored in the OpenSearch
                    var osChecksumRecord = osChecksumRecordResult.Data;

                    // Deserialize the local hash file
                    var fileChecksum = JsonConvert.DeserializeObject<AuditChecksumRecord>(File.ReadAllText(localHashFile));

                    // Check if the hash file stored in OpenSearch can be found.
                    if (osChecksumRecord == null)
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Could not find checksum record in OpenSearch for {Path.GetFileName(localHashFile)}.");
                        return result;
                    }

                    // Check if for some reason the checksum is missing in the hash file
                    // Can be a problem with the serialization or mallformed file.
                    if (fileChecksum == null || string.IsNullOrWhiteSpace(fileChecksum.Checksum))
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Checksum in the {Path.GetFileName(localHashFile)} is invalid.");
                        return result;
                    }

                    // Check if for some reason the checksum is missing in the OpenSearch for the particular hash file
                    // Can be a problem with the serialization or mallformed data.
                    if (osChecksumRecord.Checksum == null || string.IsNullOrWhiteSpace(osChecksumRecord.Checksum))
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Checksum in the hash record for {Path.GetFileName(localHashFile)} is invalid.");
                        return result;
                    }

                    // Check if we fail to recalculate the checksum based on the data in OpenSearch
                    if (string.IsNullOrWhiteSpace(osCalculatedRecordsChecksum))
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Checksum of the OpenSearch records for file {localLogFileName} is invalid.");
                        return result;
                    }

                    // Check if we fail to recalculate the checksum based on the local log file
                    if (string.IsNullOrWhiteSpace(localLogRecordsChecksum))
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Checksum of the local log records for file {localLogFileName} is invalid.");
                        return result;
                    }

                    _logger.LogDebug("osChecksumRecord.Checksum: {Stringified}", osChecksumRecord.Checksum);
                    _logger.LogDebug("fileChecksum.Checksum: {Stringified}", fileChecksum.Checksum);
                    // All checksums must match!
                    // We compare all checksums -
                    // - one that is stored in the local hash file
                    // - with the one calculated again for the corresponding local log file
                    // - with the one we get from OpenSearch or the same hash file name
                    // - with the one we calculate again based on all records we get from OpenSearch that belongs to the same local log file name
                    if (!(osCalculatedRecordsChecksum == fileChecksum.Checksum && localLogRecordsChecksum == fileChecksum.Checksum && osChecksumRecord.Checksum == fileChecksum.Checksum))
                    {
                        _logger.LogWarning(
                            "OpenSearch Records Checksum Match: {OpenSearchRecordsMatch}; " +
                            "Local logfile Records Checksum Match: {LocalLogRecordsMatch}; " +
                            "OpenSearch Hash Record Checksum Match: {OpenSearchHashMatch}",
                            osCalculatedRecordsChecksum == fileChecksum.Checksum,
                            localLogRecordsChecksum == fileChecksum.Checksum,
                            osChecksumRecord.Checksum == fileChecksum.Checksum
                        );
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Checksum mismatch for the file '{Path.GetFileName(localHashFile)}'. Log file '{fileFullPath}' and OpenSearch data.");
                        return result;
                    }

                    //Verify secure timestamp
                    var tsVerifyResult = tsProvider.VerifyToken(fileChecksum.Checksum, fileChecksum.Timestamp, tsCertChain);

                    // Timestamp token must be validated against the certificate and
                    // the secure date inside the token must be the same as the
                    // DateCreated saved in the hash file.
                    // That ensures the hash file is cerated at the time recorded in the hash file.
                    // Phisical creation date of the hash and log files will differ from the recorded secure timestamp
                    // because there is a lag between the events of file creation and retrieval of the secure timestamp 
                    // from the SignServer.
                    if (tsVerifyResult == null || !tsVerifyResult.IsValid || tsVerifyResult.TokenInfo.Timestamp.UtcDateTime != fileChecksum.DateCreated)
                    {
                        result.Status = VerificationStatus.CompletedWithErrors;
                        result.Errors.Add($"Verification of the secure timestamp failed for '{Path.GetFileName(localHashFile)}'.");
                        return result;
                    }

                    // Verify logs from OpenSearch who belongs to the particular file by recalculating record level hashes
                    VerifyEachRecordForFile(result, sortedOsRecords, localLogFile);

                }
                else
                {
                    result.Status = VerificationStatus.CompletedWithErrors;

                    result.Errors.Add($"Error trying to get the audit log file hash record from OpenSearch in hash index '{hashIndexName}', for log file '{localLogFileName}'. See next errors for more information.");

                    if (osChecksumRecordResult.Error != null)
                        result.Errors.Add(osChecksumRecordResult.Error.ToString());

                    if (!string.IsNullOrWhiteSpace(osChecksumRecordResult.DebugInformation))
                        result.Errors.Add(osChecksumRecordResult.DebugInformation);

                    return result;
                }
            }
            else
            {
                result.Status = VerificationStatus.CompletedWithErrors;

                result.Errors.Add($"Error trying to get the audit log records from OpenSearch in index '{logsIndexName}', for log file '{localLogFileName}'. See next errors for more information.");

                if (sortedOsRecordsResult.Error != null)
                    result.Errors.Add(sortedOsRecordsResult.Error.ToString());

                if (!string.IsNullOrWhiteSpace(sortedOsRecordsResult.DebugInformation))
                    result.Errors.Add(sortedOsRecordsResult.DebugInformation);

                return result;
            }

        }
        catch (Exception ex)
        {
            result.Status = VerificationStatus.ErrorAborted;

            result.Errors.Add($"Error validating record checksums for file {localLogFileName}. See next error.");
            result.Errors.Add(ex.Message);

            _logger.LogError(ex, "Error validating record checksums for file {fileName}", localLogFileName);
        }

        return result;
    }

    /// <summary>
    /// Verify the audit log record checksum
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool VerifyLogRecordChecksum(AuditLogRecord record)
    {

        string checksum = record.Checksum;
        string sourceFile = record.SourceFile;

        record.Checksum = null;
        record.SourceFile = null;

        var key = _cryptoProvider.GetKey();
        if (key == null || key.Length == 0)
            throw new Exception($"Invalid crypto key provided from the {_cryptoProvider.GetType().Name} provider.");

        var testChecksum = CalculateChecksum(record, key);

        record.Checksum = checksum;
        record.SourceFile = sourceFile;

        return testChecksum.Equals(checksum, StringComparison.Ordinal);

    }

    /// <summary>
    /// Calculates the checksum of a single audit log record.
    /// For the checksum to match it is important that the serialized string is exactly the same.
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="encryptionKey">The encryption key.</param>
    /// <returns></returns>
    public string CalculateChecksum(AuditLogRecord record, byte[] encryptionKey)
    {
        string? checksum = null;

        using (var hash = new HMACSHA512(encryptionKey))
        {
            var hashString = JsonConvert.SerializeObject(record, Formatting.None);

            var data = Encoding.UTF8.GetBytes(hashString);
            var checksumData = hash.ComputeHash(data);
            checksum = Convert.ToBase64String(checksumData);
        }

        return checksum;
    }

    /// <summary>
    /// Parses the local log file meta data.
    /// This extracts the SystemId, Log SessionId, File date and rotation index from the name of the file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    public (AuditLogFileMetaData? Metadata, List<string> Errors) ParseFileMetaData(string fileName)
    {
        var parseErrors = new List<string>();
        AuditLogFileMetaData item = new AuditLogFileMetaData();

        string errText = $"Invalid or missing part of the file name {fileName}. File name must follow the folowing pattern: SYSTEM-ID_SESSION-ID_yyyyMMddHH_NNN.EXTENSION";

        try
        {
            var fileNameParts = fileName.Split('_', '.');

            if (fileNameParts.Length < 4 || fileNameParts.Length > 5)
            {
                parseErrors.Add(errText);
                return (null, parseErrors);
            }


            item.SystemId = fileNameParts[0];

            if (string.IsNullOrWhiteSpace(item.SystemId))
            {
                parseErrors.Add(errText);
                return (null, parseErrors);
            }


            // 2023062814
            if (!DateTime.TryParseExact(fileNameParts[2], "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime timestamp))
            {
                parseErrors.Add(errText);
                return (null, parseErrors);
            }

            item.SourceFile = fileName;
            item.FileTimeStamp = timestamp;

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
            parseErrors.Add($"Error parsing the file '{fileName}' metadata. Error: {ex}");
        }

        return (null, parseErrors);
    }

    /// <summary>
    /// Finds the file in hierarchy.
    /// </summary>
    /// <param name="startDir">The start dir.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private string? FindFile(DirectoryInfo startDir, string fileName)
    {

        string location = Path.Combine(startDir.FullName, fileName);

        if (File.Exists(location))
            return location;

        var files = startDir.GetFiles(fileName, SearchOption.AllDirectories);

        if (files.Length > 0)
        {
            return files[0].FullName;
        }

        return null;
    }

    /// <summary>
    /// Loads the audit log records from the local log file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>An alphabetically sorted list by EventId of audit log records.</returns>
    /// <exception cref="System.Exception">Cannot deserialize line {lineId} from file '{fileName}'</exception>
    public static SortedList<string, AuditLogRecord> LoadRecordsFromFile(string fileName)
    {
        var result = new SortedList<string, AuditLogRecord>();

        var lines = File.ReadAllLines(fileName);
        int lineId = 0;

        foreach (var line in lines)
        {
            lineId++;

            var record = JsonConvert.DeserializeObject<AuditLogRecord>(line);
            if (record != null)
            {
                result.Add(record.EventId, record);
            }
            else
            {
                throw new Exception($"Cannot deserialize line {lineId} from file '{fileName}'");
            }
        }

        return result;
    }
    /// <summary>
    /// Loads the audit log records from the local log file asynchronous.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="cancelToken">The cancel token.</param>
    /// <returns>Task of an alphabetically sorted list by EventId of audit log records.</returns>
    /// <exception cref="System.Exception">Cannot deserialize line {lineId} from file '{fileName}'</exception>
    public async Task<SortedList<string, AuditLogRecord>> LoadRecordsFromFileAsync(string fileName, CancellationToken cancelToken)
    {
        var result = new SortedList<string, AuditLogRecord>();

        var lines = await File.ReadAllLinesAsync(fileName, cancelToken);
        int lineId = 0;

        foreach (var line in lines)
        {
            lineId++;

            var record = JsonConvert.DeserializeObject<AuditLogRecord>(line);
            if (record != null)
            {
                result.Add(record.EventId, record);
            }
            else
            {
                throw new Exception($"Cannot deserialize line {lineId} from file '{fileName}'");
            }
        }

        return result;
    }

    /// <summary>Verifies each record for the log file.</summary>
    /// <param name="result">The result.</param>
    /// <param name="sortedLocalLogRecords">The sorted local log records.</param>
    /// <param name="fileName">Name of the file.</param>
    public void VerifyEachRecordForFile(FileVerificationResult result, SortedList<string, AuditLogRecord> sortedLocalLogRecords, string fileName)
    {
        // Verify each record if on file level everything is ok.
        // At this point it is proven that both sources of logs has one an the same EventIds.
        // Then verify if the checksums match and all local logs are in place.
        foreach (var record in sortedLocalLogRecords)
        {
            // Create an object thet will hold the result of the verification process on the record level.
            var recordResult = new RecordVerificationResult();

            // Calculate the checksum again for the record in local log to see if it will generate the same result.
            recordResult.IsChecksumMatch = VerifyLogRecordChecksum(record.Value);

            // Find the local log record for the same EventId
            var localLog = sortedLocalLogRecords[record.Value.EventId];

            recordResult.EventId = record.Value.EventId;

            if (localLog == null)
            {
                // We couldn't find the corresponding local log record for the one in OpenSearch.
                // Integrity is broken because the local log file is altered.
                result.Status = VerificationStatus.CompletedWithErrors;
                result.Errors.Add($"Could not find event with EventId '{record.Value.EventId}' in the log file '{fileName}'");
                recordResult.RecordMatchLocalLogs = false;
                recordResult.LocalLogRecordExists = false;

                result.InvalidRecords.Add(recordResult);
            }
            else
            {
                // Check if record checksum saved in the record match the one recalculated for the same record
                if (!recordResult.IsChecksumMatch)
                {
                    result.Status = VerificationStatus.CompletedWithErrors;
                    result.Errors.Add($"Record checksums does not match for EventId '{record.Value.EventId}' in the log file '{fileName}'");
                    recordResult.RecordMatchLocalLogs = false;
                    recordResult.LocalLogRecordExists = true;

                    result.InvalidRecords.Add(recordResult);
                }
            }

        }

    }

    public void SaveVerificationStates(VerificationServiceState state, StatefulServiceSettingsBase settings)
    {
        if (string.IsNullOrWhiteSpace(settings.StatePath))
            throw new ArgumentException(nameof(settings));

        var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new StringEnumConverter());

        string? savePath = Path.GetDirectoryName(settings.StatePath);

        if (!string.IsNullOrWhiteSpace(savePath) && !Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string fullResultFileName = Path.Combine(Path.GetFullPath(settings.StatePath), "verification-svc.json");

        if (settings.KeepStateHistory)
            fullResultFileName = Path.Combine(Path.GetFullPath(settings.StatePath), $"verification-svc_{timeStamp}.json");

        state.ResultLogFile = fullResultFileName;

        var fullJsonState = JsonConvert.SerializeObject(state, Formatting.Indented, jsonSettings);

        File.WriteAllText(fullResultFileName, fullJsonState);

        state.ResultLogFileSize = new FileInfo(fullResultFileName).Length;

        foreach (var system in state.Systems)
        {
            string systemFileName = Path.Combine(Path.GetFullPath(settings.StatePath), $"verification-svc_{system.SystemId.ToLower()}_{timeStamp}.json");

            system.ResultLogFile = systemFileName;

            var jsonState = JsonConvert.SerializeObject(system, Formatting.Indented, jsonSettings);

            File.WriteAllText(systemFileName, jsonState);

            system.ResultLogFileSize = new FileInfo(systemFileName).Length;
        }

    }
}



