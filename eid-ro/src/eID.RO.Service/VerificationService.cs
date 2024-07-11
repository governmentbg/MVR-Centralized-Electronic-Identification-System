using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Requests;
using eID.RO.Service.Responses;
using eID.RO.Service.Validators;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
            var response = await pollyTR.ExecuteAsync(() => _httpClient.GetAsync(getUri));
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

            var result = CalculateVerificationResult(message, actualStateResult);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when getting actual state from TR. Request: GET {Url}", getUri);
            return UnhandledException<LegalEntityVerificationResult>();
        }
    }

    public async Task<ServiceResult<LegalEntityStateOfPlay>> GetBulstatStateOfPlayByUidAsync(string uid)
    {
        // Validation
        var validator = new GetBulstatStateOfPlayByUidValidator();
        var validationResult = await validator.ValidateAsync(uid);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CheckLegalEntityInNTR), validationResult);
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
            var response = await pollyTR.ExecuteAsync(() => _httpClient.GetAsync(getUri));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting state of play from Bulstat response: {Response}", responseStr);
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

    public async Task<ServiceResult<LegalEntityBulstatVerificationResult>> CheckLegalEntityInBulstatAsync(CheckLegalEntityInBulstatRequest request)
    {
        // Validation
        var validator = new CheckLegalEntityInBulstatRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CheckLegalEntityInNTR), validationResult);
            return BadRequest<LegalEntityBulstatVerificationResult>(validationResult.Errors);
        }

        var distinctAuthorizerUids = request.AuthorizerUids.ToHashSet(new AuthorizerUidDataEqualityComparer());
        var legalEntityStateOfPlayServiceResult = await GetBulstatStateOfPlayByUidAsync(request.Uid);
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

            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
        }

        // Merge all collections containing potential authorizers
        var distinctBulstatPersonList = new HashSet<IAuthorizerUidData>(new AuthorizerUidDataEqualityComparer());
        if (stateOfPlay.CollectiveBodies != null)
        {
            var collectiveBodies = stateOfPlay.Partners
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
            _logger.LogInformation("Not all authorizers for Uid {Uid} are present in Bulstat.", request.Uid);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
        }

        var eventTypeCode = stateOfPlay.Event?.EventType?.Code ?? string.Empty;
        if (string.IsNullOrWhiteSpace(eventTypeCode) || _deniedEventTypes.ContainsKey(eventTypeCode))
        {
            _logger.LogInformation("Uid {Uid} had bad event type {EventTypeCode}", request.Uid, eventTypeCode);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = !string.IsNullOrWhiteSpace(eventTypeCode) ? _deniedEventTypes[eventTypeCode] : EmpowermentsDenialReason.BulstatCheckFailed });

        }

        var stateCode = stateOfPlay.State?.State?.Code ?? string.Empty;
        if (stateCode != StateCodeОperating)
        {
            var denialReason = EmpowermentsDenialReason.BulstatCheckFailed;
            if (_deniedStateCodes.ContainsKey(stateCode))
            {
                denialReason = _deniedStateCodes[stateCode];
            }
            _logger.LogInformation("Uid {Uid} had bad state code {StateCode}", request.Uid, stateCode);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = denialReason });
        }

        var entryTypeCode = stateOfPlay.Event?.EntryType?.Code ?? string.Empty;
        if (_deniedEntryTypes.ContainsKey(entryTypeCode))
        {
            _logger.LogInformation("Uid {Uid} had bad entry type {EntryTypeCode}", request.Uid, entryTypeCode);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = _deniedEntryTypes[entryTypeCode] });
        }
        if (!_allowedEntryTypes.Contains(entryTypeCode))
        {
            _logger.LogInformation("Uid {Uid} entry type {EntryTypeCode} not in allowed entry types", request.Uid, entryTypeCode);
            return Ok(new LegalEntityBulstatVerificationResult { DenialReason = EmpowermentsDenialReason.BulstatCheckFailed });
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CheckUidsRestrictions), validationResult);
            return BadRequest<UidsRestrictionsResult>(validationResult.Errors);
        }

        // Action
        foreach (var uidRecord in message.Uids)
        {
            var maskedUid = Regex.Replace(uidRecord.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            var uid = uidRecord.Uid;
            var uidType = uidRecord.UidType;
            if (uidType == IdentifierType.EGN && !ValidatorHelpers.IsLawfulAge(uid))
            {
                _logger.LogInformation("Local identity {Uid} was detected as age lower than 18.", maskedUid);
                return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.BelowLawfulAge });
            }

            var queryString = new Dictionary<string, string>()
            {
                { "PersonalId", uid},
                { "UidType", uidType.ToString() }
            };

            _logger.LogInformation("Start getting Date of Death and Date of Prohibition from PIVR.");

            string dateOfDeathResponseStr;
            HttpResponseMessage dateOfDeathResponse;
            var policy = ApplicationPolicyRegistry.GetRapidOrNormalRetryPolicy(_logger, message.RapidRetries);
            var getDateOfDeathUri = QueryHelpers.AddQueryString("/api/v1/dateofdeath", queryString);
            _logger.LogInformation("Requesting Date of Death.");
            try
            {
                dateOfDeathResponse = await policy.ExecuteAsync(() => _httpClient.GetAsync(getDateOfDeathUri));
                dateOfDeathResponseStr = await dateOfDeathResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during restrictions check. Request: GET {UrlDoD}", getDateOfDeathUri.Replace(uid, maskedUid));
                return UnhandledException<UidsRestrictionsResult>();
            }

            if (!dateOfDeathResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting Date of Death from PIVR response: {Response}", dateOfDeathResponseStr);
                _logger.LogInformation("Verification for date of death for this person {Uid} was not successful.", maskedUid);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            _logger.LogInformation("Getting Date of Death completed successfully");
            var dateOfDeathResult = JsonConvert.DeserializeObject<DateResponse>(dateOfDeathResponseStr) ?? new DateResponse();

            // This person is deceased. There is date of death set
            if (dateOfDeathResult.Date != null)
            {
                _logger.LogInformation("Person {Uid} was detected as deceased.", maskedUid);
                return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.DeceasedUid });
            }

            string dateOfProhibitionResponseStr;
            HttpResponseMessage dateOfProhibitionResponse;
            var getDateOfProhibitionUri = QueryHelpers.AddQueryString("/api/v1/dateofprohibition", queryString);
            _logger.LogInformation("Requesting Date of Prohibition.");
            try
            {
                dateOfProhibitionResponse = await policy.ExecuteAsync(() => _httpClient.GetAsync(getDateOfProhibitionUri));
                dateOfProhibitionResponseStr = await dateOfProhibitionResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during restrictions check. Request: GET {UrlDoPr}", getDateOfProhibitionUri.Replace(uid, maskedUid));
                return UnhandledException<UidsRestrictionsResult>();
            }

            if (!dateOfProhibitionResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting Date of Prohibition from PIVR response: {Response}", dateOfProhibitionResponseStr);
                _logger.LogInformation("Verification for date of prohibition for this person {Uid} was not successful.", maskedUid);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            _logger.LogInformation("Getting Date of Prohibition completed successfully");
            var dateOfProhibitionResult = JsonConvert.DeserializeObject<DateResponse>(dateOfProhibitionResponseStr) ?? new DateResponse();

            // Тhis person is deceased. There is date of death set
            if (dateOfProhibitionResult.Date != null)
            {
                _logger.LogInformation("Person {uid} was detected as prohibited.", maskedUid);
                return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.ProhibitedUid });
            }

            // Foreign person can own EGN and LNCh
            // The result of check can be:
            //  - "0100" - No data for this person. It is a local person, no foreign person
            //  - "0000" - a foreign person
            var (isActualStateException, isUnsuccessfulActualState, foreignIdentityInfoResponse) = await GetForeignIdentityV2Async(uid, uidType, maskedUid);
            if (isActualStateException)
            {
                return UnhandledException<UidsRestrictionsResult>();
            }
            if (isUnsuccessfulActualState)
            {
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            if (foreignIdentityInfoResponse.ReturnInformations.ReturnCode != RegiXReturnCode.NotFound &&
                foreignIdentityInfoResponse.ReturnInformations.ReturnCode != RegiXReturnCode.OK)
            {
                _logger.LogInformation("Check foreign identity {Uid} exited with message {ReturnInformationsInfo}.", maskedUid, foreignIdentityInfoResponse.ReturnInformations.Info);
                return Ok(new UidsRestrictionsResult
                {
                    DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                });
            }

            // The foreign person not found
            if (uidType == IdentifierType.LNCh && foreignIdentityInfoResponse.ReturnInformations.ReturnCode == RegiXReturnCode.NotFound)
            {
                _logger.LogInformation("Foreign identity {Uid} is not found.", maskedUid);
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
                    _logger.LogInformation("Foreign identity {Uid} was detected as no permit {TypeOfPermit}", maskedUid, foreignIdentityInfoResponse.IdentityDocument.RPTypeOfPermit);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.NoPermit,
                        DenialReasonDescription = foreignIdentityInfoResponse.IdentityDocument.RPTypeOfPermit
                    });
                }

                if (foreignIdentityInfoResponse.DeathDateSpecified)
                {
                    _logger.LogInformation("Foreign identity {Uid} was detected as deceased.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.DeceasedUid });
                }

                if (!DateTime.TryParse(foreignIdentityInfoResponse.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                {
                    _logger.LogInformation("Birth date {BirthDate} of foreign identity {Uid} can not be parsed.", foreignIdentityInfoResponse.BirthDate, maskedUid);
                    return Ok(new UidsRestrictionsResult
                    {
                        DenialReason = EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck
                    });
                }

                if (birthDate.AddYears(18) > DateTime.UtcNow)
                {
                    _logger.LogInformation("Foreign identity {Uid} was detected as age lower than 18.", maskedUid);
                    return Ok(new UidsRestrictionsResult { DenialReason = EmpowermentsDenialReason.BelowLawfulAge });
                }
            }

            _logger.LogInformation("Verification for date of death and existing prohibition for all people was successful. No restrictions were detected.");
            return Ok(new UidsRestrictionsResult { Successfull = true });
        }

        _logger.LogInformation("Verification for date of death and existing prohibition for all people was successful. No restrictions were detected.");
        return Ok(new UidsRestrictionsResult { Successfull = true });
    }

    /// <summary>
    /// Verifies: 1. Detached signature signs the original file content, 2. Certificate contains the expected uid
    /// </summary>
    /// <param name="originalFile">Original file as text</param>
    /// <param name="signature">Base64 encoded detached signature p7s file</param>
    /// <param name="uid">Citizen EGN/LNCH</param>
    /// <returns></returns>
    public async Task<ServiceResult> VerifySignatureAsync(string originalFile, string signature, string uid, IdentifierType uidType, SignatureProvider signatureProvider)
    {
        if (string.IsNullOrWhiteSpace(originalFile))
        {
            _logger.LogInformation("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(originalFile));
            return BadRequest(nameof(originalFile), "Cannot be null or whitespace");
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogInformation("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(signature));
            return BadRequest(nameof(signature), "Cannot be null or whitespace");
        }

        if (string.IsNullOrWhiteSpace(uid))
        {
            _logger.LogInformation("{MethodName} called without {ParamName}", nameof(VerifySignatureAsync), nameof(uid));
            return BadRequest(nameof(uid), "Cannot be null or whitespace");
        }

        _logger.LogInformation("Start validating signature...");

        var polly = ApplicationPolicyRegistry.GetRetryPolicy(_logger);
        var url = $"{_httpClient.BaseAddress}api/v1/Verify/signature";
        try
        {
            var response = await polly.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
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
                        Encoding.UTF8, "application/json")
            }));

            if (!response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Failed signature validation. ({StatusCode}) {Response}", response.StatusCode.ToString(), responseStr);
                return BadRequest("Failed signature validation", $"({response.StatusCode}) {responseStr}");
            }

            _logger.LogInformation("Signature validation completed successfully");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error when validating signature. Request: POST {Url}", url);
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(VerifyUidsLawfulAge), validationResult);
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
                    _logger.LogInformation("Local identity {Uid} was detected as age lower than 18.", maskedUid);
                    return Ok(false);
                }

                continue;
            }

            _logger.LogInformation("Start getting actual state for LNCh from MVR...");
            var (actualStateExcepiton, unsuccessfulActualState, actualState) = await GetForeignIdentityV2Async(uid, IdentifierType.LNCh, maskedUid);
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
                _logger.LogInformation("Check foreign identity {Uid} exited with message {ReturnInformationsInfo}.", maskedUid, actualState.ReturnInformations.Info);
                // 20231205 PK I want to discuss this logic.
                //return Ok(true);

                return NotFound<bool>("ReturnInformationsInfo", actualState.ReturnInformations.Info);
            }

            if (!DateTime.TryParse(actualState.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
            {
                _logger.LogInformation("Birth date {BirthDate} of foreign identity {Uid} can not be parsed.", actualState.BirthDate, maskedUid);
                return Ok(false);
            }

            if (birthDate.AddYears(18) > DateTime.UtcNow)
            {
                _logger.LogInformation("Foreign identity {Uid} was detected as age lower than 18.", maskedUid);
                return Ok(false);
            }
        }

        _logger.LogInformation("Verification for date of death and existing prohibition for all people was successful. No restrictions were detected.");
        return Ok(true);
    }

    private async Task<(bool, bool, ForeignIdentityInfoResponseType)> GetForeignIdentityV2Async(string uid, IdentifierType identifierType, string maskedUid)
    {
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
            httpResponse = await pollyMVR.ExecuteAsync(() => _httpClient.GetAsync(getUri));
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

    public static LegalEntityVerificationResult CalculateVerificationResult(CheckLegalEntityInNTR message, LegalEntityActualState actualState)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (actualState is null)
        {
            throw new ArgumentNullException(nameof(actualState));
        }

        var actualStateResponse = actualState.Response?.ActualStateResponseV3;

        if (message.Name != actualStateResponse?.Deed?.CompanyName ||
            message.Uid != actualStateResponse.Deed.UIC)
        {
            //Check is not OK here
            //Successfull = false; 
            return new LegalEntityVerificationResult();
        }

        var representative1 = actualStateResponse?.Deed?.Subdeeds?.Subdeed?.FirstOrDefault()?.Records?.Record.FirstOrDefault(r => r.MainField?.MainFieldIdent == _representativeField1Id);
        var representative2 = actualStateResponse?.Deed?.Subdeeds?.Subdeed?.FirstOrDefault()?.Records?.Record.FirstOrDefault(r => r.MainField?.MainFieldIdent == _representativeField2Id);
        var representative3 = actualStateResponse?.Deed?.Subdeeds?.Subdeed?.FirstOrDefault()?.Records?.Record.FirstOrDefault(r => r.MainField?.MainFieldIdent == _representativeField3Id);

        var wayOfRepresentation = actualStateResponse?.Deed?.Subdeeds?.Subdeed?.FirstOrDefault()?.Records?.Record.FirstOrDefault(r => r.MainField?.MainFieldIdent == _wayOfRepresentationFieldId);

        if (wayOfRepresentation == null && //field 00110 is missing
            representative1 != null && representative1.RecordData != null)
        {
            var repPerson1 = GetRepresentative(representative1);

            if (CheckRepresentativeData(repPerson1, message.IssuerUid, message.IssuerName))
            {
                return new LegalEntityVerificationResult(true, GetUserIdentifiers(new Person?[] { repPerson1?.Subject }));
            }
            else//Person name and/or egn are not matching: Successfull = false;
            {
                return new LegalEntityVerificationResult();
            }
        }
        else
        {
            Representative repPerson1 = GetRepresentative(representative1);
            Representative repPerson2 = GetRepresentative(representative2);
            Representative repPerson3 = GetRepresentative(representative3);

            if (wayOfRepresentation != null && wayOfRepresentation.RecordData != null)
            {
                var wOrD = wayOfRepresentation.RecordData["wayOfRepresentation"];
                var wOr = new WayOfRepresentation();
                if (wOrD != null)
                {
                    wOr = JsonConvert.DeserializeObject<WayOfRepresentation>(wOrD.ToString()) ?? new WayOfRepresentation();
                }

                if (wOr.Jointly || wOr.OtherWay)
                {
                    if (CheckRepresentativeData(repPerson1, message.IssuerUid, message.IssuerName) ||
                       CheckRepresentativeData(repPerson2, message.IssuerUid, message.IssuerName) ||
                       CheckRepresentativeData(repPerson3, message.IssuerUid, message.IssuerName))
                    {
                        var subjects = new[] { repPerson1?.Subject, repPerson2?.Subject, repPerson3?.Subject };
                        var uIds = GetUserIdentifiers(subjects);

                        return new LegalEntityVerificationResult(true, uIds);
                    }
                    else//no one of the Person name and/or egn are not matching: Successfull = false;
                    {
                        return new LegalEntityVerificationResult();
                    }
                }
                else //wOr.Severally
                {
                    if (CheckRepresentativeData(repPerson1, message.IssuerUid, message.IssuerName))
                    {
                        return new LegalEntityVerificationResult(true, GetUserIdentifiers(new Person?[] { repPerson1?.Subject }));
                    }

                    if (CheckRepresentativeData(repPerson2, message.IssuerUid, message.IssuerName))
                    {
                        return new LegalEntityVerificationResult(true, GetUserIdentifiers(new Person?[] { repPerson2?.Subject }));
                    }

                    if (CheckRepresentativeData(repPerson3, message.IssuerUid, message.IssuerName))
                    {
                        return new LegalEntityVerificationResult(true, GetUserIdentifiers(new Person?[] { repPerson3?.Subject }));
                    }
                }
            }

            //no one is matching
            return new LegalEntityVerificationResult();
        }
    }

    private static Representative GetRepresentative(RecordItem? item)
    {
        if (item != null && item.RecordData != null)
        {
            var rep = item.RecordData["representative"];
            if (rep != null)
            {
                return JsonConvert.DeserializeObject<Representative>(rep.ToString()) ?? new Representative();
            }
        }

        return new Representative();
    }


    private static IEnumerable<UserIdentifierData> GetUserIdentifiers(Person?[] people)
    {
        foreach (var person in people)
        {
            if (person != null)
            {
                if (!Enum.TryParse<IdentifierType>(person.IndentType, true, out var identityType))
                { identityType = IdentifierType.NotSpecified; }

                yield return new UserIdentifierData { Uid = person.Indent, UidType = identityType };
            }
        }
    }
    private static bool CheckRepresentativeData(Representative repPerson, string uId, string name)
    {
        return repPerson != null &&
               repPerson.Subject != null &&
               repPerson.Subject.Indent == uId &&
               repPerson.Subject.Name?.ToUpperInvariant() == name.ToUpperInvariant();
    }

    public async Task<ServiceResult<LegalEntityActualState>> GetLegalEntityActualStateAsync(string uid)
    {
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
            var response = await pollyTR.ExecuteAsync(() => _httpClient.GetAsync(getUri));
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
