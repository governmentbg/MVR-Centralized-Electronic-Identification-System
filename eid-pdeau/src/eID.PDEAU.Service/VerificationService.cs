using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Requests;
using eID.PDEAU.Service.Responses;
using eID.PDEAU.Service.Validators;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.PDEAU.Service;

public class VerificationService : BaseService, IVerificationService
{
    private readonly ILogger<VerificationService> _logger;
    private readonly IDistributedCache _cache;
    private readonly HttpClient _pivrHttpClient;

    private const string StateNotFound = "0100";
    private const string StateCodeОperating = "571";

    private const string StateCodeInitialRegistration = "756";
    private const string StateCodeChangeInCircumstances = "757";
    private readonly string[] _allowedEntryTypes = new string[] { StateCodeInitialRegistration, StateCodeChangeInCircumstances };

    // Вид събитие
    private readonly List<string> _deniedEventTypes = new()
    {
         "563",  //ClosedInBulstat
         "564",  //TerminatedThroughMergerInBulstat
         "565",  //TerminatedThroughIncorporationInBulstat
         "566",  //TerminatedThroughDivisionInBulstat
         "568",  //DeletedFromJudicialRegister
         "569",  //DeletedFromBTPPRegister
         "570",  //RegistrationAnnulledInBulstat
         "1071"  //TerminatedDueToEnterpriseTransactionInBulstat
    };

    // Видове вписване
    private readonly List<string> _deniedEntryTypes = new()
    {
        "758" //EmpowermentsDenialReason.DeregisteredInBulstat
    };

    public VerificationService(
       ILogger<VerificationService> logger,
       IDistributedCache cache,
       IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _pivrHttpClient = httpClientFactory.CreateClient("PIVR");
    }

    public async Task<ServiceResult<DeauRequesterInBulstatVerificationResult>> CheckDeauRequesterInBulstatAsync(CheckDeauRequesterInBulstatRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validation
        var validator = new CheckDeauRequesterInBulstatRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CheckDeauRequesterInBulstatAsync), validationResult);
            return BadRequest<DeauRequesterInBulstatVerificationResult>(validationResult.Errors);
        }

        var distinctAuthorizerUids = new HashSet<IAuthorizerUidData>(new[] { request.AuthorizerData }, new AuthorizerUidDataEqualityComparer());
        var legalEntityStateOfPlayServiceResult = await GetBulstatStateOfPlayByUidAsync(request.CorrelationId, request.Uid);
        var stateOfPlay = legalEntityStateOfPlayServiceResult?.Result?.Response?.StateOfPlayResponseType;
        if (legalEntityStateOfPlayServiceResult is null
            || legalEntityStateOfPlayServiceResult?.StatusCode != System.Net.HttpStatusCode.OK
            || stateOfPlay is null)
        {
            _logger.LogInformation(
                "Malformed StateOfPlay response. StatusCode {StatusCode}; Service Result is null {ServiceResultIsNull}; StateOfPlay is null {StateOfPlayIsNull}",
                legalEntityStateOfPlayServiceResult?.StatusCode,
                legalEntityStateOfPlayServiceResult is null,
                stateOfPlay is null
            );

            return Ok(new DeauRequesterInBulstatVerificationResult());
        }

        if (legalEntityStateOfPlayServiceResult?.Result?.Response?.StateOfPlayResponseType?.ReturnInformations?.ReturnCode == StateNotFound)
        {
            _logger.LogInformation("Cannot find subject in Bulstat with Uid: {Uid}", request.Uid);
            return Ok(new DeauRequesterInBulstatVerificationResult());
        }

        // Merge all collections containing potential authorizers
        var distinctBulstatPersonList = new HashSet<IAuthorizerUidData>(new AuthorizerUidDataEqualityComparer());
        if (stateOfPlay.CollectiveBodies != null)
        {
            var members = stateOfPlay.CollectiveBodies.SelectMany(cb => cb.Members).Where(m => m != null);
            if (members.Any())
            {
                var collectiveBodies = members
                        .Select(s => new AuthorizerUidData
                        {
                            Name = s.RelatedSubject.NaturalPersonSubject.CyrillicName,
                            Uid = s.RelatedSubject.NaturalPersonSubject.EGN ?? s.RelatedSubject.NaturalPersonSubject.LNC,
                            UidType = s.RelatedSubject.NaturalPersonSubject.EGN != null ? IdentifierType.EGN : IdentifierType.LNCh
                        });
                distinctBulstatPersonList.UnionWith(collectiveBodies);
            }
        }

        if (stateOfPlay.Managers != null)
        {
            var managers = stateOfPlay.Managers
                    .Select(s => new AuthorizerUidData
                    {
                        Name = s.RelatedSubject.NaturalPersonSubject.CyrillicName,
                        Uid = s.RelatedSubject.NaturalPersonSubject.EGN ?? s.RelatedSubject.NaturalPersonSubject.LNC,
                        UidType = s.RelatedSubject.NaturalPersonSubject.EGN != null ? IdentifierType.EGN : IdentifierType.LNCh
                    });
            distinctBulstatPersonList.UnionWith(managers);
        }
        if (stateOfPlay.Partners != null)
        {
            var partners = stateOfPlay.Partners
                    .Select(s => new AuthorizerUidData
                    {
                        Name = s.RelatedSubject.NaturalPersonSubject.CyrillicName,
                        Uid = s.RelatedSubject.NaturalPersonSubject.EGN ?? s.RelatedSubject.NaturalPersonSubject.LNC,
                        UidType = s.RelatedSubject.NaturalPersonSubject.EGN != null ? IdentifierType.EGN : IdentifierType.LNCh
                    });
            distinctBulstatPersonList.UnionWith(partners);
        }

        var issuerPresentAmongAuthorizers = distinctBulstatPersonList.Any(x => x.Uid == request.AuthorizerData.Uid);
        if (!issuerPresentAmongAuthorizers)
        {
            _logger.LogInformation("Authorizer: {AuthorizerUid} not present among representatives in Bulstat.", request.AuthorizerData);
            return Ok(new DeauRequesterInBulstatVerificationResult());
        }

        var eventTypeCode = stateOfPlay.Event?.EventType?.Code ?? string.Empty;
        if (string.IsNullOrWhiteSpace(eventTypeCode) || _deniedEventTypes.Contains(eventTypeCode))
        {
            _logger.LogInformation("Uid {Uid} had bad event type {EventTypeCode}", request.Uid, eventTypeCode);
            return Ok(new DeauRequesterInBulstatVerificationResult());

        }

        var stateCode = stateOfPlay.State?.State?.Code ?? string.Empty;
        if (stateCode != StateCodeОperating)
        {
            _logger.LogInformation("Uid {Uid} had bad state code {StateCode}", request.Uid, stateCode);
            return Ok(new DeauRequesterInBulstatVerificationResult());
        }

        var entryTypeCode = stateOfPlay.Event?.EntryType?.Code ?? string.Empty;
        if (_deniedEntryTypes.Contains(entryTypeCode))
        {
            _logger.LogInformation("Uid {Uid} had bad entry type {EntryTypeCode}", request.Uid, entryTypeCode);
            return Ok(new DeauRequesterInBulstatVerificationResult());
        }
        if (!_allowedEntryTypes.Contains(entryTypeCode))
        {
            _logger.LogInformation("Uid {Uid} entry type {EntryTypeCode} not in allowed entry types", request.Uid, entryTypeCode);
            return Ok(new DeauRequesterInBulstatVerificationResult());
        }

        _logger.LogInformation("Uid {Uid} succeeded all Bulstat checks.", request.Uid);
        return Ok(new DeauRequesterInBulstatVerificationResult { Successful = true });
    }

    public async Task<ServiceResult<LegalEntityStateOfPlay>> GetBulstatStateOfPlayByUidAsync(Guid correlationId, string uid)
    {
        // Validation
        var validator = new GetBulstatStateOfPlayByUidValidator();
        var validationResult = await validator.ValidateAsync((correlationId, uid));
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetBulstatStateOfPlayByUidAsync), validationResult);
            return BadRequest<LegalEntityStateOfPlay>(validationResult.Errors);
        }

        // Action
        var getStateOfPlayUrl = "/api/v1/registries/bulstat/getstateofplay";
        var queryString = new Dictionary<string, string>()
                {
                    { "UIC", uid }
                };
        var getUri = QueryHelpers.AddQueryString(getStateOfPlayUrl, queryString);

        _logger.LogInformation("Start getting state of play from Bulstat");

        var pollyTR = ApplicationPolicyRegistry.GetCallRegiXPolicy(_logger, "BULSTAT");

        try
        {
            var response = await pollyTR.ExecuteAsync(() =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation(Contracts.Constants.HeaderNames.RequestId, correlationId.ToString());
                return _pivrHttpClient.SendAsync(requestMessage);
            });

            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed getting state of play from Bulstat response: {Response}", responseStr);
            }

            var stateOfPlay = JsonConvert.DeserializeObject<LegalEntityStateOfPlay>(responseStr) ?? new LegalEntityStateOfPlay();

            _logger.LogInformation("Getting state of play from Bulstat completed successfully");

            if (stateOfPlay.HasFailed || stateOfPlay.Response is null)
            {
                return Ok(new LegalEntityStateOfPlay());
            }
            return Ok(stateOfPlay);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when getting state of play from Bulstat. Request: GET {Url}", getUri);
            return UnhandledException<LegalEntityStateOfPlay>();
        }
    }
}

public interface IVerificationService
{
    Task<ServiceResult<DeauRequesterInBulstatVerificationResult>> CheckDeauRequesterInBulstatAsync(CheckDeauRequesterInBulstatRequest request);
}

