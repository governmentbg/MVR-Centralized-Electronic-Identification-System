using eID.PJS.Contracts.Commands;
using eID.PJS.Contracts.Results;
using eID.PJS.Service.Validators;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace eID.PJS.Service;

public class OpenSearchCollectorService : BaseService
{
    private readonly ILogger<OpenSearchCollectorService> _logger;
    private readonly IOpenSearchClient _openSearchClient;

    public OpenSearchCollectorService(
        ILogger<OpenSearchCollectorService> logger,
        IOpenSearchClient openSearchClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _openSearchClient = openSearchClient ?? throw new ArgumentNullException(nameof(openSearchClient));
    }

    public async Task<ServiceResult<CursorResult<LogUserFromMeResult>>> GetLogUserFromMeAsync(GetLogUserFromMe message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogUserFromMeValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogUserFromMe), validationResult);
            return BadRequest<CursorResult<LogUserFromMeResult>>(validationResult.Errors);
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

        // Action
        var shouldQueries = new List<Func<QueryContainerDescriptor<LogUserFromMeResult>, QueryContainer>>();
        if (message.UserUidType != Contracts.IdentifierType.NotSpecified)
        {
            shouldQueries.Add(q =>
                q.Bool(b =>
                    b.Should(
                        s => s.Bool(
                            bb => bb.Must(
                                bs => bs.Term(t => t
                                    .Field("eventPayload.TargetUidType")
                                    .Value(message.UserUidType.ToString())
                                ),
                                bs => bs.Term(t => t
                                            .Field("eventPayload.TargetUid")
                                            .Value(EncryptionHelper.Encrypt(message.UserUid))
                                        )
                                )),
                        s => s.Bool(
                            bb => bb.MustNot(
                                mn => mn.Exists(e => e.Field("eventPayload.TargetUidType")),
                                mn => mn.Exists(e => e.Field("eventPayload.TargetUid"))
                            )
                        )
                    )
            ));
        }

        var mustQueries = new List<Func<QueryContainerDescriptor<LogUserFromMeResult>, QueryContainer>>();
        mustQueries.Add(bs => bs.Term(t => t
                            .Field("requesterUserId")
                            .Value(EncryptionHelper.Encrypt(message.UserId))
                        ));

        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));

        mustQueries.Add(bs => bs.Terms(t => t
                            .Field(f => f.EventType)
                            .Terms(message.EventTypes)
                        ));

        var mustNotQueries = new List<Func<QueryContainerDescriptor<LogUserFromMeResult>, QueryContainer>>();
        if (message.ExcludedEventTypes != null && message.ExcludedEventTypes.Any())
        {
            mustNotQueries.Add(mnq => mnq.Terms(t => t
                .Field(f => f.EventType)
                .Terms(message.ExcludedEventTypes)
            ));
        }

        var searchDescriptor = new SearchDescriptor<LogUserFromMeResult>()
            .Size(message.CursorSize)
            .Query(q => q
                .Bool(b => b
                    .Should(shouldQueries)
                    .Must(mustQueries)
                    .MustNot(mustNotQueries)
                )
            )
            .Sort(srt => srt
                .Descending(fld => fld.EventDate));

        var result = await GetSearchCursorResultAsync(searchDescriptor, message.CursorSearchAfter);
        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogUserFromMeAsync), message.UserId);
        return Ok(result);
    }

    public async Task<ServiceResult<CursorResult<LogUserToMeResult>>> GetLogUserToMeAsync(GetLogUserToMe message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogUserToMeValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogUserToMe), validationResult);
            return BadRequest<CursorResult<LogUserToMeResult>>(validationResult.Errors);
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
        var shouldQueries = new List<Func<QueryContainerDescriptor<LogUserToMeResult>, QueryContainer>>();
        if (message.UserUidType != Contracts.IdentifierType.NotSpecified)
        {
            shouldQueries.Add(q =>
                q.Bool(b => b.Must(
                        bs => bs.Term(t => t
                            .Field("eventPayload.TargetUidType")
                            .Value(message.UserUidType.ToString())
                        ),
                        bs => bs.Term(t => t
                                    .Field("eventPayload.TargetUid")
                                    .Value(EncryptionHelper.Encrypt(message.UserUid))
                                )
                )
            ));
        }

        if (!string.IsNullOrWhiteSpace(message.UserEid))
        {
            shouldQueries.Add(q => q.Bool(b => b
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

        shouldQueries.Add(bs => bs.Term(t => t
                            .Field("targetUserId")
                            .Value(EncryptionHelper.Encrypt(message.UserId))
                        ));

        var mustQueries = new List<Func<QueryContainerDescriptor<LogUserToMeResult>, QueryContainer>>();
        mustQueries.Add(bs => bs.DateRange(dr => dr
                            .Field(f => f.EventDate)
                            .GreaterThanOrEquals(startDate)
                            .LessThanOrEquals(endDate)
                        ));

        mustQueries.Add(bs => bs.Terms(t => t
                            .Field(f => f.EventType)
                            .Terms(message.EventTypes)
                        ));

        var mustNotQueries = new List<Func<QueryContainerDescriptor<LogUserToMeResult>, QueryContainer>>();
        mustNotQueries.Add(mnq => mnq.Bool(b =>
                            b.Must(
                                m => m.Term(t => t.Field("requesterUserId").Value(EncryptionHelper.Encrypt(message.UserId))),
                                m => m.Exists(e => e.Field("requesterUserId"))
                            )
                        ));
        if (message.ExcludedEventTypes != null && message.ExcludedEventTypes.Any())
        {
            mustNotQueries.Add(mnq => mnq.Terms(t => t
                .Field(f => f.EventType)
                .Terms(message.ExcludedEventTypes)
            ));
        }

        var searchDescriptor = new SearchDescriptor<LogUserToMeResult>()
            .Size(message.CursorSize)
            .Query(q => q
                .Bool(b => b
                    //.MinimumShouldMatch(1)
                    .Should(shouldQueries)
                    .Must(mustQueries)
                    .MustNot(mustNotQueries)
                )
            )
            .Sort(srt => srt
                .Descending(fld => fld.EventDate)
        );

        var result = await GetSearchCursorResultAsync(searchDescriptor, message.CursorSearchAfter);

        _logger.LogInformation("Executed {MethodName} for {UserId}", nameof(GetLogUserToMeAsync), message.UserId);
        return Ok(result);
    }

    public async Task<ServiceResult<CursorResult<LogDeauResult>>> GetLogDeauAsync(GetLogDeau message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetLogDeauValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetLogDeau), validationResult);
            return BadRequest<CursorResult<LogDeauResult>>(validationResult.Errors);
        }

        var mustQueries = new List<Func<QueryContainerDescriptor<LogDeauResult>, QueryContainer>>();

        var startDate = message.StartDate?.ToUniversalTime();
        var endDate = message.EndDate?.ToUniversalTime();

        // Add DateRange only if at least one valid date is provided
        var dateRangeQuery = BuildDateRangeQuery(startDate, endDate);
        if (dateRangeQuery is not null)
            mustQueries.Add(dateRangeQuery);

        // Add remaining required filters
        mustQueries.Add(q => q.Terms(t => t
            .Field(f => f.EventType)
            .Terms(message.EventTypes)));

        mustQueries.Add(q => q.Terms(t => t
            .Field("requesterSystemId")
            .Terms(message.SystemId)));

        // Action
        var searchDescriptor = new SearchDescriptor<LogDeauResult>()
            .Size(message.CursorSize)
            .Query(q => q.Bool(b => b.Must(mustQueries)))
            .Sort(s => s.Descending(f => f.EventDate));

        var result = await GetSearchCursorResultAsync(searchDescriptor, message.CursorSearchAfter);
        _logger.LogInformation("Executed {MethodName} for {SystemId}", nameof(GetLogDeauAsync), message.SystemId);
        return Ok(result);
    }

    private static Func<QueryContainerDescriptor<LogDeauResult>, QueryContainer>? BuildDateRangeQuery(DateTime? start, DateTime? end)
    {
        if (!start.HasValue && !end.HasValue)
            return null;

        return q => q.DateRange(dr =>
        {
            dr = dr.Field(f => f.EventDate);
            if (start.HasValue) dr = dr.GreaterThanOrEquals(start.Value);
            if (end.HasValue) dr = dr.LessThanOrEquals(end.Value);
            return dr;
        });
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
