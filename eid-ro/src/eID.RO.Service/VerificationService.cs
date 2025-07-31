using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Requests;
using eID.RO.Service.Responses;
using eID.RO.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;

namespace eID.RO.Service;

public class VerificationService : BaseService, IVerificationService
{
    private const string _representativeField1Id = "00100";
    private const string _representativeField2Id = "00101";
    private const string _representativeField3Id = "00102";
    private const string _wayOfRepresentationFieldId = "00110";
    private const string StateCodeОperating = "571";
    private const string StateCodeInitialRegistration = "756";
    private const string StateCodeChangeInCircumstances = "757";
    private readonly string[] _allowedEntryTypes = new string[] { StateCodeInitialRegistration, StateCodeChangeInCircumstances };
    // Състояние
    private readonly Dictionary<string, EmpowermentsDenialReason> _deniedStateCodes = new()
    {
        { "1", EmpowermentsDenialReason.ReregisteredInNTR },
        { "2", EmpowermentsDenialReason.ArchivedInBulstat },
        { "572", EmpowermentsDenialReason.InInsolvencyProceedingsInBulstat },
        { "573", EmpowermentsDenialReason.InsolventInBulstat },
        { "574", EmpowermentsDenialReason.InLiquidationInBulstat },
        { "575", EmpowermentsDenialReason.InactiveInBulstat },
    };
    // Видове вписване
    private readonly Dictionary<string, EmpowermentsDenialReason> _deniedEntryTypes = new()
    {
        { "758", EmpowermentsDenialReason.DeregisteredInBulstat },
    };
    // Вид събитие
    private readonly Dictionary<string, EmpowermentsDenialReason> _deniedEventTypes = new()
    {
        { "563", EmpowermentsDenialReason.ClosedInBulstat },
        { "564", EmpowermentsDenialReason.TerminatedThroughMergerInBulstat },
        { "565", EmpowermentsDenialReason.TerminatedThroughIncorporationInBulstat },
        { "566", EmpowermentsDenialReason.TerminatedThroughDivisionInBulstat },
        { "568", EmpowermentsDenialReason.DeletedFromJudicialRegister },
        { "569", EmpowermentsDenialReason.DeletedFromBTPPRegister },
        { "570", EmpowermentsDenialReason.RegistrationAnnulledInBulstat },
        { "1071", EmpowermentsDenialReason.TerminatedDueToEnterpriseTransactionInBulstat }
    };

    private readonly ILogger<VerificationService> _logger;
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly IMpozeiCaller _mpozeiCaller;
    private readonly IConfiguration _configuration;

    private readonly HashSet<string> _foreignPersonAllowedPermits = new HashSet<string>(new CaseInsensitiveStringEqualityComparer())
    {
        "КРАТКОСРОЧНО ПРЕБИВАВАЩ В РБ",
        "ПРОДЪЛЖИТЕЛНО ПРЕБИВАВАЩ В РБ",
        "ПОСТОЯННО ПРЕБИВАВАЩ В РБ",
        "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ",
        "БЕЖАНЕЦ",
        "ЧУЖДЕНЕЦ С ХУМАНИТАРЕН СТАТУТ",
        "ПРЕДОСТАВЕНО УБЕЖИЩЕ",
        "ВРЕМЕННА ЗАКРИЛА"
    };

    public VerificationService(
        ILogger<VerificationService> logger,
        IDistributedCache cache,
        HttpClient httpClient,
        IMpozeiCaller mpozeiCaller,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _mpozeiCaller = mpozeiCaller ?? throw new ArgumentNullException(nameof(mpozeiCaller));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<ServiceResult<LegalEntityVerificationResult>> VerifyRequesterInLegalEntityAsync(CheckLegalEntityInNTR message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CheckLegalEntityInNTRValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CheckLegalEntityInNTR), validationResult);
            return BadRequest<LegalEntityVerificationResult>(validationResult.Errors);
        }

        // Action
        var getActualStateUrl = "/api/v1/registries/tr/getactualstatev3";
        var queryString = new Dictionary<string, string>()
                {
                    { "UIC", message.Uid }
                };
        var getUri = QueryHelpers.AddQueryString(getActualStateUrl, queryString);

        _logger.LogInformation("Start getting actual state for legal entity from TR...");

        var pollyTR = ApplicationPolicyRegistry.GetCallRegiXPolicy(_logger, "TR");

        try
        {
            var response = await pollyTR.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", message.CorrelationId.ToString());

                return await _httpClient.SendAsync(requestMessage);
            });
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting actual state from TR response: {Response}", responseStr);
            }

            var actualStateResult = JsonConvert.DeserializeObject<LegalEntityActualState>(responseStr) ?? new LegalEntityActualState();

            _logger.LogInformation("Getting actual state for legal entity from TR completed successfully");

            if (actualStateResult.HasFailed || actualStateResult.Response is null)
            {
                return Ok(new LegalEntityVerificationResult());
            }

            var correctCompanyNameAndUic = actualStateResult.MatchCompanyData(message.Name, message.Uid);
            var requesterInLegalEntity = actualStateResult.ContainsRepresentativesData() && actualStateResult.IsAmongRepresentatives(message.IssuerUid, message.IssuerName);
            var authorizerUids = Enumerable.Empty<UserIdentifier>();
            if (actualStateResult.ContainsRepresentativesData())
            {
                authorizerUids = actualStateResult.Response.ActualStateResponseV3.GetRepresentatives()
                    .Select(u =>
                    {
                        if (!Enum.TryParse<IdentifierType>(u.IndentType, true, out var identityType))
                        { identityType = IdentifierType.NotSpecified; }

                        return new UserIdentifierData { Uid = u.Indent, UidType = identityType };
                    });
            }
            return Ok(new LegalEntityVerificationResult
            {
                Successful = correctCompanyNameAndUic && requesterInLegalEntity,
                AuthorizerUids = authorizerUids
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when getting actual state from TR. Request: GET {Url}", getUri);
            return UnhandledException<LegalEntityVerificationResult>();
        }
    }

    public async Task<ServiceResult<LegalEntityStateOfPlay>> GetBulstatStateOfPlayByUidAsync(Guid correlationId, string uid)
    {
        // Validation
        var validator = new GetBulstatStateOfPlayByUidValidator();
        var validationResult = await validator.ValidateAsync((correlationId, uid));
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(CheckLegalEntityInNTR), validationResult);
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
            var response = await pollyTR.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());

                return await _httpClient.SendAsync(requestMessage);
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
            _logger.LogError(ex, "Error when getting state of play from Bulstat. Request: GET {Url}", getUri);
            return UnhandledException<LegalEntityStateOfPlay>();
        }
    }

    public async Task<ServiceResult<LegalEntityBulstatVerificationResult>> CheckLegalEntityInBulstatAsync(CheckLegalEntityInBulstatRequest request)
    {
        // Validation
        var validator = new CheckLegalEntityInBulstatRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(CheckLegalEntityInNTR), validationResult);
            return BadRequest<LegalEntityBulstatVerificationResult>(validationResult.Errors);
        }

        var distinctAuthorizerUids = request.AuthorizerUids.ToHashSet(new AuthorizerUidDataEqualityComparer());
        var legalEntityStateOfPlayServiceResult = await GetBulstatStateOfPlayByUidAsync(request.CorrelationId, request.Uid);
        var stateOfPlay = legalEntityStateOfPlayServiceResult?.Result?.Response?.StateOfPlayResponseType;
        if (legalEntityStateOfPlayServiceResult is null
            || legalEntityStateOfPlayServiceResult?.StatusCode != System.Net.HttpStatusCode.OK
            || stateOfPlay is null)
        {
            _logger.LogWarning(
                "Malformed StateOfPlay response. StatusCode {StatusCode}; Service Result is null {ServiceResultIsNull}; StateOfPlay is null {StateOfPlayIsNull}",
                legalEntityStateOfPlayServiceResult?.StatusCode,
                legalEntityStateOfPlayServiceResult is null,
                stateOfPlay is null
            );

            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
        }

        // Merge all collections containing potential authorizers
        var distinctBulstatPersonList = new HashSet<IAuthorizerUidData>(new AuthorizerUidDataEqualityComparer());
        if (stateOfPlay.CollectiveBodies != null)
        {
            var collectiveBodies = stateOfPlay.CollectiveBodies
                    .Where(cb => cb.Members != null)
                    .SelectMany(s => s.Members.Where(m => m != null).Select(m => m))
                    .Select(s => new AuthorizerUidData
                    {
                        Name = s.RelatedSubject.NaturalPersonSubject.CyrillicName,
                        Uid = s.RelatedSubject.NaturalPersonSubject.EGN ?? s.RelatedSubject.NaturalPersonSubject.LNC,
                        UidType = s.RelatedSubject.NaturalPersonSubject.EGN != null ? IdentifierType.EGN : IdentifierType.LNCh
                    });
            distinctBulstatPersonList.UnionWith(collectiveBodies);
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

        var notAllAuthorizersArePresentInBulstat = !distinctAuthorizerUids.IsSubsetOf(distinctBulstatPersonList);
        if (notAllAuthorizersArePresentInBulstat)
        {
            _logger.LogWarning("Not all authorizers for Uid {Uid} are present in Bulstat.", request.Uid);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
        }

        // Only check if present
        if (stateOfPlay.Event?.EventType?.Code is not null)
        {
            var eventTypeCode = stateOfPlay.Event.EventType.Code;
            if (_deniedEventTypes.ContainsKey(eventTypeCode))
            {
                _logger.LogWarning("Uid {Uid} had bad event type {EventTypeCode}", request.Uid, eventTypeCode);
                return Ok(new LegalEntityBulstatVerificationResult { DenialReason = !string.IsNullOrWhiteSpace(eventTypeCode) ? _deniedEventTypes[eventTypeCode] : EmpowermentsDenialReason.BulstatCheckFailed });
            }
        }

        var stateCode = stateOfPlay.State?.State?.Code ?? string.Empty;
        if (stateCode != StateCodeОperating)
        {
            var denialReason = EmpowermentsDenialReason.BulstatCheckFailed;
            if (_deniedStateCodes.ContainsKey(stateCode))
            {
                denialReason = _deniedStateCodes[stateCode];
            }
            _logger.LogWarning("Uid {Uid} had bad state code {StateCode}", request.Uid, stateCode);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = denialReason });
        }

        // Only check if present
        if (stateOfPlay.Event?.EntryType?.Code is not null)
        {
            var entryTypeCode = stateOfPlay.Event.EntryType.Code;
            if (_deniedEntryTypes.ContainsKey(entryTypeCode))
            {
                _logger.LogWarning("Uid {Uid} had bad entry type {EntryTypeCode}", request.Uid, entryTypeCode);
                return Ok(new LegalEntityBulstatVerificationResult { DenialReason = _deniedEntryTypes[entryTypeCode] });
            }
            if (!_allowedEntryTypes.Contains(entryTypeCode))
            {
                _logger.LogWarning("Uid {Uid} entry type {EntryTypeCode} not in allowed entry types", request.Uid, entryTypeCode);
                return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
            }
        }

        _logger.LogInformation("Uid {Uid} succeeded all Bulstat checks.", request.Uid);
        return Ok(new LegalEntityBulstatVerificationResult { Successful = true });
    }

    /// <summary>
    /// Checks the list of Authorized Uids for prohibition and deceased information.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>A result of the check</returns>
    public async Task<ServiceResult<UidsRestrictionsResult>> CheckUidsRestrictionsAsync(CheckUidsRestrictions message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CheckUidsRestrictionsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(CheckUidsRestrictions), validationResult);
            return BadRequest<UidsRestrictionsResult>(validationResult.Errors);
        }
        var useNaif = _configuration.GetValue<bool>("USE_NAIF_FOR_RESTRICTIONS");
        // Action
        foreach (var uidRecord in message.Uids)
        {
            var maskedUid = Regex.Replace(uidRecord.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            var uid = uidRecord.Uid;
            var uidType = uidRecord.UidType;
            if (uidType == IdentifierType.EGN && !ValidatorHelpers.IsLawfulAge(uid))
            {
                _logger.LogWarning("Local identity {Uid} was detected as age lower than 18.", maskedUid);
                return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.BelowLawfulAge });
            }

            var policy = ApplicationPolicyRegistry.GetRapidOrNormalRetryPolicy(_logger, message.RapidRetries);
            if (useNaif)
            {
                _logger.LogInformation("Start getting Date of Death and Date of Prohibition from NAIF.");
                var naifResult = await CheckUidForRestrictionsAsync(message.CorrelationId, uid, maskedUid, new Dictionary<string, string>()
                {
                    { "Uid", uid},
                    { "UidType", uidType.ToString() } // It's important to remain string as integer values are not correlating
                }, policy);
                if (naifResult.HasFailed)
                {
                    _logger.LogWarning("Failed NAIF call for {Uid}.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck });
                }
                var naifResponse = naifResult.Response;
                if (naifResponse.IsDead)
                {
                    _logger.LogWarning("Person {Uid} was detected as deceased.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.DeceasedUid });
                }

                if (naifResponse.IsProhibited)
                {
                    _logger.LogWarning("Person {uid} was detected as prohibited.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.ProhibitedUid });
                }
            }
            else
            {
                _logger.LogInformation("Start getting Date of Death and Date of Prohibition from PIVR.");

                var queryString = new Dictionary<string, string>()
                {
                    { "PersonalId", uid},
                    { "UidType", uidType.ToString() }
                };
                var dateOfDeathResult = await GetDateOfDeathAsync(message.CorrelationId, uid, maskedUid, queryString, policy);
                if (dateOfDeathResult is null)
                {
                    _logger.LogWarning("Failed DateOfDeath call for {Uid}.", maskedUid);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                    });
                }
                // This person is deceased. There is date of death set
                if (dateOfDeathResult.Date != null)
                {
                    _logger.LogWarning("Person {Uid} was detected as deceased.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.DeceasedUid });
                }

                var dateOfProhibitionResult = await GetDateOfProhibitionAsync(message.CorrelationId, uid, maskedUid, queryString, policy);
                if (dateOfProhibitionResult is null)
                {
                    _logger.LogWarning("Failed DateOfProhibition call for {Uid}.", maskedUid);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                    });
                }
                // Тhis person is deceased. There is date of death set
                if (dateOfProhibitionResult.Date != null)
                {
                    _logger.LogWarning("Person {uid} was detected as prohibited.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.ProhibitedUid });
                }
            }

            // Foreign person can own EGN and LNCh
            // The result of check can be:
            //  - "0100" - No data for this person. It is a local person, no foreign person
            //  - "0000" - a foreign person
            var (isActualStateException, isUnsuccessfulActualState, foreignIdentityInfoResponse) = await GetForeignIdentityV2Async(message.CorrelationId, uid, uidType, maskedUid);
            if (isActualStateException)
            {
                _logger.LogWarning("Failed GetForeignIdentityV2 call for {Uid}.", maskedUid);
                return UnhandledException<UidsRestrictionsResult>();
            }
            if (isUnsuccessfulActualState)
            {
                _logger.LogWarning("Unsuccessful GetForeignIdentityV2 call for {Uid}.", maskedUid);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            if (foreignIdentityInfoResponse.ReturnInformations.ReturnCode != RegiXReturnCode.NotFound &&
                foreignIdentityInfoResponse.ReturnInformations.ReturnCode != RegiXReturnCode.OK)
            {
                _logger.LogWarning("Check foreign identity {Uid} exited with message {ReturnInformationsInfo}.", maskedUid, foreignIdentityInfoResponse.ReturnInformations.Info);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            // The foreign person not found
            if (uidType == IdentifierType.LNCh && foreignIdentityInfoResponse.ReturnInformations.ReturnCode == RegiXReturnCode.NotFound)
            {
                _logger.LogWarning("Foreign identity {Uid} is not found.", maskedUid);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            // It is a foreign person, does not matter it owns EGN or LNCh
            if (foreignIdentityInfoResponse.ReturnInformations.ReturnCode == RegiXReturnCode.OK)
            {
                if (!_foreignPersonAllowedPermits.Contains(foreignIdentityInfoResponse.IdentityDocument.RPTypeOfPermit))
                {
                    _logger.LogWarning("Foreign identity {Uid} was detected as no permit {TypeOfPermit}", maskedUid, foreignIdentityInfoResponse.IdentityDocument.RPTypeOfPermit);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.NoPermit,
                        DenialReasonDescription = foreignIdentityInfoResponse.IdentityDocument.RPTypeOfPermit
                    });
                }

                if (foreignIdentityInfoResponse.DeathDateSpecified)
                {
                    _logger.LogWarning("Foreign identity {Uid} was detected as deceased.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.DeceasedUid });
                }

                if (!DateTime.TryParse(foreignIdentityInfoResponse.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                {
                    _logger.LogWarning("Birth date {BirthDate} of foreign identity {Uid} can not be parsed.", foreignIdentityInfoResponse.BirthDate, maskedUid);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                    });
                }

                if (birthDate.AddYears(18) > DateTime.UtcNow)
                {
                    _logger.LogWarning("Foreign identity {Uid} was detected as age lower than 18.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.BelowLawfulAge });
                }
            }

            _logger.LogInformation("Verification for date of death and existing prohibition for {Uid} was successful. No restrictions were detected.", maskedUid);
        }

        _logger.LogInformation("Verification for date of death and existing prohibition for all people was successful. No restrictions were detected.");
        return Ok(new UidsRestrictionsResult { Successful = true });
    }

    private async Task<DateResponse?> GetDateOfDeathAsync(Guid correlationId, string uid, string maskedUid, Dictionary<string, string> queryString, IAsyncPolicy<HttpResponseMessage> policy)
    {
        string dateOfDeathResponseStr;
        HttpResponseMessage dateOfDeathResponse;
        var getDateOfDeathUri = QueryHelpers.AddQueryString("/api/v1/dateofdeath", queryString);
        _logger.LogInformation("Requesting Date of Death.");

        try
        {
            dateOfDeathResponse = await policy.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getDateOfDeathUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());
                return await _httpClient.SendAsync(requestMessage);
            });
            dateOfDeathResponseStr = await dateOfDeathResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during restrictions check. Request: GET {UrlDoD}", getDateOfDeathUri.Replace(uid, maskedUid));
            return null;
        }

        if (!dateOfDeathResponse.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed getting Date of Death from PIVR response: {Response}", dateOfDeathResponseStr);
            _logger.LogInformation("Verification for date of death for this person {Uid} was not successful.", maskedUid);
            return null;
        }

        _logger.LogInformation("Getting Date of Death completed successfully");
        return JsonConvert.DeserializeObject<DateResponse>(dateOfDeathResponseStr) ?? new DateResponse();
    }

    private async Task<DateResponse?> GetDateOfProhibitionAsync(Guid correlationId, string uid, string maskedUid, Dictionary<string, string> queryString, IAsyncPolicy<HttpResponseMessage> policy)
    {
        string dateOfProhibitionResponseStr;
        HttpResponseMessage dateOfProhibitionResponse;
        var getDateOfProhibitionUri = QueryHelpers.AddQueryString("/api/v1/dateofprohibition", queryString);
        _logger.LogInformation("Requesting Date of Prohibition.");

        try
        {
            dateOfProhibitionResponse = await policy.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getDateOfProhibitionUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());
                return await _httpClient.SendAsync(requestMessage);
            });
            dateOfProhibitionResponseStr = await dateOfProhibitionResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during restrictions check. Request: GET {UrlDoPr}", getDateOfProhibitionUri.Replace(uid, maskedUid));
            return null;
        }

        if (!dateOfProhibitionResponse.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed getting Date of Prohibition from PIVR response: {Response}", dateOfProhibitionResponseStr);
            _logger.LogInformation("Verification for date of prohibition for this person {Uid} was not successful.", maskedUid);
            return null;
        }

        _logger.LogInformation("Getting Date of Prohibition completed successfully");
        return JsonConvert.DeserializeObject<DateResponse>(dateOfProhibitionResponseStr) ?? new DateResponse();
    }

    private async Task<CheckUidRestrictionsResult> CheckUidForRestrictionsAsync(Guid correlationId, string uid, string maskedUid, Dictionary<string, string> queryString, IAsyncPolicy<HttpResponseMessage> policy)
    {
        string responseStr;
        HttpResponseMessage response;
        var getUri = QueryHelpers.AddQueryString("/api/v1/registries/otherservices/checkuidrestrictions", queryString);
        _logger.LogInformation("Checking uid restrictions in NAIF.");

        try
        {
            response = await policy.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());
                return await _httpClient.SendAsync(requestMessage);
            });
            responseStr = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during restrictions check. Request: GET {URL}", getUri.Replace(uid, maskedUid));
            return new CheckUidRestrictionsResponse { HasFailed = true, Error = "Exception during checking for restrictions" };
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed checking uid for restrictions. ({StatusCode}) {Response}", response.StatusCode, responseStr);
            _logger.LogInformation("Checking uid for restrictions for {Uid} was not successful.", maskedUid);
            return new CheckUidRestrictionsResponse { HasFailed = true, Error = "Unsuccessful check for restrictions" };
        }

        _logger.LogInformation("Checking uid for restrictions completed successfully");
        try
        {
            return JsonConvert.DeserializeObject<CheckUidRestrictionsResponse>(responseStr) ?? new CheckUidRestrictionsResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deserialization of restrictions check. Response: {ResponseStr}", responseStr.Replace(uid, maskedUid));
            return new CheckUidRestrictionsResponse();
        }
    }


    /// <summary>
    /// Verifies: 1. Detached signature signs the original file content, 2. Certificate contains the expected uid
    /// </summary>
    /// <param name="originalFile">Original file as text</param>
    /// <param name="signature">Base64 encoded detached signature p7s file</param>
    /// <param name="uid">Citizen EGN/LNCH</param>
    /// <returns></returns>
    public async Task<ServiceResult> VerifySignatureAsync(Guid correlationId, string originalFile, string signature, string uid, IdentifierType uidType, SignatureProvider signatureProvider)
    {
        if (Guid.Empty == correlationId)
        {
            _logger.LogWarning("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(correlationId));
            return BadRequest(nameof(correlationId), "Cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(originalFile))
        {
            _logger.LogWarning("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(originalFile));
            return BadRequest(nameof(originalFile), "Cannot be null or whitespace");
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogWarning("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(signature));
            return BadRequest(nameof(signature), "Cannot be null or whitespace");
        }

        if (string.IsNullOrWhiteSpace(uid))
        {
            _logger.LogWarning("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(uid));
            return BadRequest(nameof(uid), "Cannot be null or whitespace");
        }

        _logger.LogInformation("Start validating signature...");

        var polly = ApplicationPolicyRegistry.GetRetryPolicy(_logger);
        var url = $"{_httpClient.BaseAddress}api/v1/Verify/signature";
        try
        {
            var response = await polly.ExecuteAsync(() =>
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new StringContent(
                                    JsonConvert.SerializeObject(new
                                    {
                                        OriginalFile = originalFile,
                                        DetachedSignature = signature,
                                        Uid = uid,
                                        SignatureProvider = signatureProvider,
                                        UidType = uidType
                                    },
                                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                                        Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                request.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());
                return _httpClient.SendAsync(request);
            });

            if (!response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed signature validation. ({StatusCode}) {Response}", response.StatusCode.ToString(), responseStr);

                var validationProblemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseStr);
                if (validationProblemDetails?.Errors?.Any() ?? false)
                {
                    return BadRequest(validationProblemDetails.Errors);
                }

                return BadRequest("Failed signature validation", $"({response.StatusCode}) {responseStr}");
            }

            _logger.LogInformation("Signature validation completed successfully");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when validating signature. Request: POST {Url}", url);
            return UnhandledException();
        }
    }

    public async Task<ServiceResult<bool>> VerifyUidsLawfulAgeAsync(VerifyUidsLawfulAge message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new VerifyUidsLawfulAgeValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(VerifyUidsLawfulAge), validationResult);
            return BadRequest<bool>(validationResult.Errors);
        }

        // By default:
        // EGN is validated via checksum AND lawful age
        // LNCH is validated via checksum ONLY
        // Invalid Uid is any uid that failed both EGN and LNCH validations. It would've been caught by now.
        // By filtering invalid EGN formats we're leaving only LNCHs
        foreach (var userIdentifier in message.Uids)
        {
            var maskedUid = Regex.Replace(userIdentifier.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            var uid = userIdentifier.Uid;
            var uidType = userIdentifier.UidType;
            if (uidType == IdentifierType.EGN)
            {
                if (!ValidatorHelpers.IsLawfulAge(uid))
                {
                    _logger.LogWarning("Local identity {Uid} was detected as age lower than 18.", maskedUid);
                    return Ok(false);
                }

                continue;
            }

            _logger.LogInformation("Start getting actual state for LNCh from MVR...");
            var (actualStateExcepiton, unsuccessfulActualState, actualState) = await GetForeignIdentityV2Async(message.CorrelationId, uid, IdentifierType.LNCh, maskedUid);
            if (actualStateExcepiton)
            {
                return UnhandledException<bool>();
            }
            if (unsuccessfulActualState)
            {
                return Ok(false);
            }

            _logger.LogInformation("Getting response for foreign identity from MVR completed successfully");
            if (actualState.ReturnInformations.ReturnCode != RegiXReturnCode.OK)
            {
                // If there is no such LNCH found, we won't stop the process
                _logger.LogWarning("Check foreign identity {Uid} exited with message {ReturnInformationsInfo}.", maskedUid, actualState.ReturnInformations.Info);
                // 20231205 PK I want to discuss this logic.
                //return Ok(true);

                return NotFound<bool>("ReturnInformationsInfo", actualState.ReturnInformations.Info);
            }

            if (!actualState.BirthDateParsed.HasValue)
            {
                _logger.LogWarning("Birth date {BirthDate} of foreign identity {Uid} can not be parsed.", actualState.BirthDate, maskedUid);
                return Ok(false);
            }

            if (actualState.BirthDateParsed.Value.AddYears(18) > DateTime.UtcNow)
            {
                _logger.LogInformation("Foreign identity {Uid} was detected as age lower than 18.", maskedUid);
                return Ok(false);
            }
        }

        _logger.LogInformation("Verification lawful age for all people was successful.");
        return Ok(true);
    }
    public async Task<ServiceResult<bool>> VerifyUidsRegistrationStatusAsync(VerifyUidsRegistrationStatus message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new VerifyUidsRegistrationStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(VerifyUidsLawfulAge), validationResult);
            return BadRequest<bool>(validationResult.Errors);
        }

        var invalidRegistrationUid = string.Empty;
        var errors = new List<KeyValuePair<string, string>>();
        foreach (var userIdentifier in message.Uids)
        {
            var maskedUid = Regex.Replace(userIdentifier.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            var uid = userIdentifier.Uid;
            var uidType = userIdentifier.UidType;

            var userProfile = await _mpozeiCaller.FetchUserProfileAsync(message.CorrelationId, uid, uidType);
            // There are citizens with two names. That's why we omit empty fields when we build the name
            var joinedProfileName = string.Join(" ", new List<string> {
                                                        userProfile?.FirstName, userProfile?.SecondName, userProfile?.LastName }
                                                        .Where(s => !string.IsNullOrWhiteSpace(s)));
            var namesDoNotMatch = !userIdentifier.Name.ToUpperInvariant().Equals(joinedProfileName.ToUpperInvariant());
            var profileNotFound = userProfile is not null && string.IsNullOrWhiteSpace(userProfile.EidentityId) && string.IsNullOrWhiteSpace(userProfile.CitizenProfileId) && (!userProfile.Active); // No registration
            var profileIsInvalid = userProfile is null // Failed request
                || profileNotFound
                || !userProfile.Active // Uid registration was deactivated
                || string.IsNullOrWhiteSpace(userProfile.CitizenProfileId) // No base profile
                || namesDoNotMatch; // No names match

            if (profileIsInvalid)
            {
                invalidRegistrationUid = maskedUid;
                _logger.LogWarning("Invalid registration detected for {EmpowermentId}; MaskedUid: {MaskedUid}; ProfileIsNull: {ProfileIsNull}; Profile not found {ProfileNotFound}; Active profile: {ActiveProfile}; Citizen Profile Id: {CitizenProfileId}; NamesDoNotMatch: {NamesDoNotMatch}",
                    message.EmpowermentId,
                    maskedUid,
                    userProfile is null,  //Failed request
                    profileNotFound,
                    userProfile?.Active, // Uid registration was deactivated
                    userProfile?.CitizenProfileId, // No base profile
                    namesDoNotMatch); // No names match
                // 1. Profile not found
                if (profileNotFound)
                {
                    errors.Add(new KeyValuePair<string, string>("NoRegistration", "User profile not found."));
                }
                // 2. Failed request - userProfile is null
                else if (userProfile is null)
                {
                    errors.Add(new KeyValuePair<string, string>("ConnectionFailure", "Failed user profile check request."));
                }
                else
                {
                    // 3. User profile was deactivated
                    if (userProfile.Active == false)
                    {
                        errors.Add(new KeyValuePair<string, string>("InactiveProfile", "User profile is deactivated."));
                    }

                    // 4. No base profile
                    if (string.IsNullOrWhiteSpace(userProfile.CitizenProfileId))
                    {
                        errors.Add(new KeyValuePair<string, string>("NoBaseProfile", "Missing base citizen profile ID."));
                    }

                    // 5. Names do not match
                    if (namesDoNotMatch)
                    {
                        errors.Add(new KeyValuePair<string, string>("NamesMismatch", "Provided names do not match the registered profile."));
                    }
                }
                break;
            }
        }
        if (!string.IsNullOrWhiteSpace(invalidRegistrationUid))
        {
            _logger.LogWarning("Invalid registration detected for {EmpowermentId}; MaskedUid: {MaskedUid}", message.EmpowermentId, invalidRegistrationUid);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = errors
            };
        }

        _logger.LogInformation("Verification for valid registrations for all uids of empowerment {EmpowermentId} was successful.", message.EmpowermentId);
        return Ok(true);
    }

    private async Task<(bool, bool, ForeignIdentityInfoResponseType)> GetForeignIdentityV2Async(Guid correlationId, string uid, IdentifierType identifierType, string maskedUid)
    {
        if (Guid.Empty == correlationId)
        {
            throw new ArgumentException($"'{nameof(correlationId)}' cannot be empty.", nameof(correlationId));
        }

        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException($"'{nameof(uid)}' cannot be null or whitespace.", nameof(uid));
        }

        if (identifierType == IdentifierType.NotSpecified)
        {
            throw new ArgumentException($"'{nameof(identifierType)}' cannot be '{IdentifierType.NotSpecified}'.", nameof(identifierType));
        }

        _logger.LogInformation("Start getting actual state for LNCh from MVR...");
        var queryString = new Dictionary<string, string>()
        {
            { "Identifier", uid },
            { "IdentifierType", identifierType.ToString() }
        };
        var getUri = QueryHelpers.AddQueryString("/api/v1/registries/mvr/getforeignidentityv2", queryString);

        var pollyMVR = ApplicationPolicyRegistry.GetCallRegiXPolicy(_logger, "MVR");

        HttpResponseMessage httpResponse;
        string responseStr = string.Empty;
        try
        {
            httpResponse = await pollyMVR.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());

                return await _httpClient.SendAsync(requestMessage);
            });
            responseStr = await httpResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when getting actual state for foreign identity from MVR during {MethodName}.", nameof(GetForeignIdentityV2Async));

            return (true, false, new ForeignIdentityInfoResponseType());
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed getting actual state for foreign identity from MVR response: {Response}", responseStr);

            return (false, true, new ForeignIdentityInfoResponseType());
        }

        var actualState = JsonConvert.DeserializeObject<ForeignIdentityActualState>(responseStr) ?? new ForeignIdentityActualState();

        if (actualState.HasFailed || actualState.Response?.ForeignIdentityInfoResponse is null)
        {
            _logger.LogInformation("Verification of foreign identity {Uid} was not successful.", maskedUid);
            return (false, true, new ForeignIdentityInfoResponseType());
        }

        return (false, false, actualState.Response.ForeignIdentityInfoResponse);
    }

    public async Task<ServiceResult<LegalEntityActualState>> GetLegalEntityActualStateAsync(Guid correlationId, string uid)
    {
        if (Guid.Empty == correlationId)
        {
            throw new ArgumentException($"'{nameof(correlationId)}' cannot be empty.", nameof(correlationId));
        }

        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException($"'{nameof(uid)}' cannot be null or empty.", nameof(uid));
        }

        // Action
        var getActualStateUrl = "/api/v1/registries/tr/getactualstatev3";
        var queryString = new Dictionary<string, string>()
                {
                    { "UIC", uid }
                };
        var getUri = QueryHelpers.AddQueryString(getActualStateUrl, queryString);

        _logger.LogInformation("Start getting actual state for legal entity from TR...");

        var pollyTR = ApplicationPolicyRegistry.GetCallRegiXPolicy(_logger, "TR");

        try
        {
            var response = await pollyTR.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, getUri);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());

                return await _httpClient.SendAsync(requestMessage);
            });
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting actual state from TR response: {Response}", responseStr);
            }

            var actualStateResult = JsonConvert.DeserializeObject<LegalEntityActualState>(responseStr) ?? new LegalEntityActualState();

            _logger.LogInformation("Getting actual state for legal entity from TR completed successfully");

            if (actualStateResult.HasFailed || actualStateResult.Response is null)
            {
                return Ok(new LegalEntityActualState());
            }

            return Ok(actualStateResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when getting actual state from TR. Request: GET {Url}", getUri);
            return UnhandledException<LegalEntityActualState>();
        }
    }
}

internal class CaseInsensitiveStringEqualityComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        return x.Equals(y, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
    {
        // Use the case-insensitive hash code to generate the hash value
        return obj.ToLowerInvariant().GetHashCode();
    }
}
