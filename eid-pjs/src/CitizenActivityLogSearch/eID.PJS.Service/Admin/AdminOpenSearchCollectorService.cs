using System.Security.Cryptography;
using System.Text;
using eID.PJS.AuditLogging;
using eID.PJS.Contracts;
using eID.PJS.Contracts.Commands;
using eID.PJS.Contracts.Commands.Admin;
using eID.PJS.Contracts.Results;
using eID.PJS.Contracts.Results.Admin;
using eID.PJS.Service.Admin.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenSearch.Client;

namespace eID.PJS.Service.Admin;

public class AdminOpenSearchCollectorService : BaseService
{
    private readonly ILogger<AdminOpenSearchCollectorService> _logger;
    private readonly IOpenSearchClient _openSearchClient;

    public AdminOpenSearchCollectorService(
        ILogger<AdminOpenSearchCollectorService> logger,
        IOpenSearchClient openSearchClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _openSearchClient = openSearchClient ?? throw new ArgumentNullException(nameof(openSearchClient));
    }

    public async Task<ServiceResult<CursorResult<LogFromUserResult>>> GetLogFromUserAsync(GetLogFromUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogFromUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogFromUser), validationResult);
            return BadRequest<CursorResult<LogFromUserResult>>(validationResult.Errors);
        }

        DateTime? startDate = null;
        if (message.StartDate.HasValue)
        {
            startDate = message.StartDate.Value.ToUniversalTime();
        }
        DateTime? endDate = null;
        if (message.EndDate.HasValue)
        {
            endDate = message.EndDate.Value.ToUniversalTime();
        }

        var mustQueries = new List<Func<QueryContainerDescriptor<LogFromUserResult>, QueryContainer>>();
        if (!string.IsNullOrWhiteSpace(message.EventId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventId")
                                .Value(message.EventId)
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.TargetName))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetName")
                                .Value(message.TargetName)
                            ));
        }
        if (message.TargetUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUidType")
                                .Value(message.TargetUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUid")
                                .Value(EncryptionHelper.Encrypt(message.TargetUid))
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserId))
        {
            mustQueries.Add(q => q.Bool(b => b
                .Should(
                    s => s.Term(t => t
                        .Field("requesterUserId")
                        .Value(EncryptionHelper.Encrypt(message.UserId))),
                    s => s.Term(t => t
                                .Field("requesterUserId")
                        .Value(EncryptionHelper.Encrypt("", allowEmptyStrings: true)))
                    )
                .MinimumShouldMatch(1)
            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserEid))
        {
            mustQueries.Add(q => q.Bool(b => b
                .Should(
                    s => s.Bool(bb => bb
                    // If eventPayload.TargetEidentityId exists and matches with our parameter: we're good
                        .Must(
                            m => m.Exists(e => e
                                .Field("eventPayload.TargetEidentityId")),
                            m => m.Term(t => t
                                .Field("eventPayload.TargetEidentityId")
                                .Value(message.UserEid))
                        ))
                    )
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserEid))
        {
            mustQueries.Add(q => q.Bool(b => b
                .Should(
                    s => s.Bool(bb => bb
                    // If eventPayload.TargetEidentityId exists and matches with our parameter: we're good
                        .Must(
                            m => m.Exists(e => e
                                .Field("eventPayload.TargetEidentityId")),
                            m => m.Term(t => t
                                .Field("eventPayload.TargetEidentityId")
                                .Value(message.UserEid))
                        ))
                )
            ));
        }
        if (message.RequesterUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUidType")
                                .Value(message.RequesterUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUid")
                                .Value(EncryptionHelper.Encrypt(message.RequesterUid))
                            ));
        }

        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));
        mustQueries.Add(bs => bs.Terms(t => t
                           .Field(f => f.EventType)
                           .Terms(message.EventTypes)
                       ));

        var searchDescriptor = new SearchDescriptor<LogFromUserResult>()
         .Size(message.CursorSize)
         .Query(q => q
             .Bool(b => b
                 .Must(mustQueries)
             )
         )
         .Sort(srt => srt
             .Descending(fld => fld.EventDate));

        var result = await GetSearchCursorResultAsync(searchDescriptor, message.CursorSearchAfter);
        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogFromUserAsync), message.UserId);
        result.Data.DecryptLogRecordData();

        return Ok(result);
    }

    public async Task<ServiceResult> GetLogFromUserAsFileAsync(GetLogFromUserAsFile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogFromUserAsFileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogFromUserAsFile), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var tempFile = message.FileFullPath + Constants.FileProcess.TempFileExtension;
        if (File.Exists(message.FileFullPath) || File.Exists(tempFile))
        {
            _logger.LogWarning("The file with name {FileName} is in process", message.FileFullPath);

            return Ok();
        }

        var path = Path.GetDirectoryName(message.FileFullPath);
        if (path == null)
        {
            _logger.LogError("The path {path} for the file {fileFullPath} could not be found", path, message.FileFullPath);
            return BadRequest(nameof(message.FileFullPath), $"The path {path} could not be found.");
        }
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                _logger.LogError("Can not create folder: {path}", path);

                return UnhandledException();
            }
        }

        DateTime startDate = message.StartDate.ToUniversalTime();
        DateTime endDate = message.EndDate.ToUniversalTime();

        var mustQueries = new List<Func<QueryContainerDescriptor<LogFromUserResult>, QueryContainer>>();
        if (!string.IsNullOrWhiteSpace(message.EventId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventId")
                                .Value(message.EventId)
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.TargetName))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetName")
                                .Value(message.TargetName)
                            ));
        }
        if (message.TargetUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUidType")
                                .Value(message.TargetUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUid")
                                .Value(EncryptionHelper.Encrypt(message.TargetUid))
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("requesterUserId")
                                .Value(EncryptionHelper.Encrypt(message.UserId))
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserEid))
        {
            mustQueries.Add(q => q.Bool(b => b
                .Should(
                    s => s.Bool(bb => bb
                    // If eventPayload.TargetEidentityId exists and matches with our parameter: we're good
                        .Must(
                            m => m.Exists(e => e
                                .Field("eventPayload.TargetEidentityId")),
                            m => m.Term(t => t
                                .Field("eventPayload.TargetEidentityId")
                                .Value(message.UserEid))
                        ))
                )
            ));
        }
        if (message.RequesterUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUidType")
                                .Value(message.RequesterUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUid")
                                .Value(EncryptionHelper.Encrypt(message.RequesterUid))
                            ));
        }

        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));
        mustQueries.Add(bs => bs.Terms(t => t
                           .Field(f => f.EventType)
                           .Terms(message.EventTypes)
                       ));

        var searchDescriptor = new SearchDescriptor<LogFromUserResult>()
            .Size(1000)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                )
            )
            .Sort(srt => srt
                .Descending(fld => fld.EventDate));

        await StoreCursorDataAsFileAsync(searchDescriptor, tempFile, message.FileFullPath);

        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogFromUserAsFileAsync), message.UserId);
        return Ok();
    }

    private async Task StoreCursorDataAsFileAsync<T>(SearchDescriptor<T> searchDescriptor, string tempFile, string fileFullPath) where T : class
    {
        try
        {
            // File is locked only for writing
            using (var file = File.Open(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using var streamWriter = new StreamWriter(file, Encoding.UTF8);

                IEnumerable<object>? cursorSearchAfter = null;
                while (true)
                {
                    var result = await GetSearchCursorResultAsync(searchDescriptor, cursorSearchAfter);
                    if (!result.Data.Any())
                    {
                        break;
                    }

                    cursorSearchAfter = result.SearchAfter;

                    foreach (var item in result.Data)
                    {
                        var line = JsonConvert.SerializeObject(item, Formatting.None);

                        await streamWriter.WriteLineAsync(line);
                    }
                }
            }

            // Rename file
            File.Move(tempFile, fileFullPath);
        }
        catch (IOException ex)
        {
            // Usually when the file exists
            _logger.LogWarning("Problem: {message} while opening file {file}", ex.Message, tempFile);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Problem: {message} during processing file {file}", ex.Message, tempFile);
            File.Delete(tempFile);

            throw;
        }
    }

    public async Task<ServiceResult<CursorResult<LogToUserResult>>> GetLogToUserAsync(GetLogToUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogToUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogUserToMe), validationResult);
            return BadRequest<CursorResult<LogToUserResult>>(validationResult.Errors);
        }

        DateTime? startDate = null;
        if (message.StartDate.HasValue)
        {
            startDate = message.StartDate.Value.ToUniversalTime();
        }
        DateTime? endDate = null;
        if (message.EndDate.HasValue)
        {
            endDate = message.EndDate.Value.ToUniversalTime();
        }

        // Action - get all logs where targetUserId != requesterUserId

        var mustQueries = new List<Func<QueryContainerDescriptor<LogToUserResult>, QueryContainer>>();
        if (!string.IsNullOrWhiteSpace(message.EventId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventId")
                                .Value(message.EventId)
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.TargetName))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetName")
                                .Value(message.TargetName)
                            ));
        }
        if (message.TargetUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUidType")
                                .Value(message.TargetUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUid")
                                .Value(EncryptionHelper.Encrypt(message.TargetUid))
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("targetUserId")
                                .Value(EncryptionHelper.Encrypt(message.UserId))
                            ));
        }
        if (message.RequesterUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUidType")
                                .Value(message.RequesterUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUid")
                                .Value(EncryptionHelper.Encrypt(message.RequesterUid))
                            ));
        }

        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));
        mustQueries.Add(bs => bs.Terms(t => t
                           .Field(f => f.EventType)
                           .Terms(message.EventTypes)
                       ));

        var searchDescriptor = new SearchDescriptor<LogToUserResult>()
            .Size(message.CursorSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                    .MustNot(
                        bs => bs.Term(t => t
                            .Field("requesterUserId")
                            .Value(EncryptionHelper.Encrypt(message.UserId))
                        )
                    )
                )
            )
            .Sort(srt => srt
                .Descending(fld => fld.EventDate)
        );

        var result = await GetSearchCursorResultAsync(searchDescriptor, message.CursorSearchAfter);

        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogToUserAsync), message.UserId);
        result.Data.DecryptLogRecordData();

        return Ok(result);
    }

    public async Task<ServiceResult> GetLogToUserAsFileAsync(GetLogToUserAsFile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogToUserAsFileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogUserToMe), validationResult);
            return BadRequest<CursorResult<LogToUserResult>>(validationResult.Errors);
        }

        var tempFile = message.FileFullPath + Constants.FileProcess.TempFileExtension;
        if (File.Exists(message.FileFullPath) || File.Exists(tempFile))
        {
            _logger.LogWarning("The file with name {FileName} is in process", message.FileFullPath);

            return Ok();
        }

        var path = Path.GetDirectoryName(message.FileFullPath);
        if (path == null)
        {
            _logger.LogError("The path {path} for the file {fileFullPath} could not be found", path, message.FileFullPath);
            return BadRequest(nameof(message.FileFullPath), $"The path {path} could not be found.");
        }
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                _logger.LogError("Can not create folder: {path}", path);

                return UnhandledException();
            }
        }

        DateTime startDate = message.StartDate.ToUniversalTime();
        DateTime endDate = message.EndDate.ToUniversalTime();

        // Action - get all logs where targetUserId != requesterUserId

        var mustQueries = new List<Func<QueryContainerDescriptor<LogToUserResult>, QueryContainer>>();
        if (!string.IsNullOrWhiteSpace(message.EventId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventId")
                                .Value(message.EventId)
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.TargetName))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetName")
                                .Value(message.TargetName)
                            ));
        }
        if (message.TargetUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUidType")
                                .Value(message.TargetUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.TargetUid")
                                .Value(EncryptionHelper.Encrypt(message.TargetUid))
                            ));
        }
        if (!string.IsNullOrWhiteSpace(message.UserId))
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("targetUserId")
                                .Value(EncryptionHelper.Encrypt(message.UserId))
                            ));
        }
        if (message.RequesterUidType != Contracts.IdentifierType.NotSpecified)
        {
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUidType")
                                .Value(message.RequesterUidType.ToString())
                            ));
            mustQueries.Add(bs => bs.Term(t => t
                                .Field("eventPayload.RequesterUid")
                                .Value(EncryptionHelper.Encrypt(message.RequesterUid))
                            ));
        }

        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));
        mustQueries.Add(bs => bs.Terms(t => t
                           .Field(f => f.EventType)
                           .Terms(message.EventTypes)
                       ));

        var searchDescriptor = new SearchDescriptor<LogToUserResult>()
            .Size(1000)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                    .MustNot(
                        bs => bs.Term(t => t
                            .Field("requesterUserId")
                            .Value(EncryptionHelper.Encrypt(message.UserId))
                        )
                    )
                )
            )
            .Sort(srt => srt
                .Descending(fld => fld.EventDate)
        );

        await StoreCursorDataAsFileAsync(searchDescriptor, tempFile, message.FileFullPath);

        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogToUserAsync), message.UserId);

        return Ok();
    }

    private async Task<CursorResult<T>> GetSearchCursorResultAsync<T>(SearchDescriptor<T> searchDescriptor, IEnumerable<object>? cursorSearchAfter)
        where T : class
    {
        if (cursorSearchAfter != null)
        {
            searchDescriptor.SearchAfter(cursorSearchAfter);
        }
        // Execute
        var response = await _openSearchClient.SearchAsync<T>(searchDescriptor);

        // Result
        return new CursorResult<T>
        {
            Data = response.Documents,
            SearchAfter = response.Hits.LastOrDefault()?.Sorts ?? Array.Empty<object>()
        };
    }
}

internal static class Extensions
{
    public static void DecryptLogRecordData(this IEnumerable<LogResult> collection)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionHelper.Key);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        var descriptor = aes.CreateDecryptor(aes.Key, aes.IV);

        foreach (var logRecord in collection)
        {
            try
            {
                logRecord.RequesterUserId = EncryptionHelper.Decrypt(logRecord.RequesterUserId, aes, descriptor);
            }
            catch
            {
                // The value wasn't encrypted. Return it as-is.
            }
            try
            {
                logRecord.TargetUserId = EncryptionHelper.Decrypt(logRecord.TargetUserId, aes, descriptor);
            }
            catch
            {
                // The value wasn't encrypted. Return it as-is.
            }
            if (logRecord.EventPayload is null)
            {
                continue;
            }
            if (logRecord.EventPayload.ContainsKey(AuditLoggingKeys.Request) && logRecord.EventPayload[AuditLoggingKeys.Request] is not null)
            {
                try
                {
                    logRecord.EventPayload[AuditLoggingKeys.Request] = EncryptionHelper.Decrypt(logRecord.EventPayload[AuditLoggingKeys.Request].ToString(), aes, descriptor);
                }
                catch
                {
                    // The value wasn't encrypted. Return it as-is.
                }
            }
            foreach (var key in AuditLoggingKeys.GetEncryptablePayloadKeys())
            {
                if (logRecord.EventPayload.ContainsKey(key) && logRecord.EventPayload[key] is not null)
                {
                    try
                    {
                        logRecord.EventPayload[key] = EncryptionHelper.Decrypt(logRecord.EventPayload[key].ToString(), aes, descriptor);
                    }
                    catch
                    {
                        // The value wasn't encrypted. Return it as-is.
                    }
                }
            }
        }
    }
}
