using eID.PIVR.API.Authorization;
using eID.PIVR.API.Requests;
using eID.PIVR.Contracts;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Enums;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service;
using eID.PIVR.Service.RegiXResponses;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eID.PIVR.API.Controllers;

public class RegistriesController : BaseV1Controller
{
    private const string StringParamType = "STRING";
    private readonly IApiUsageTrackerService _usageTracker;

    public RegistriesController(IConfiguration configuration, ILogger<RegistriesController> logger, AuditLogger auditLogger, IApiUsageTrackerService usageTracker) : base(configuration, logger, auditLogger)
    {
        _usageTracker = usageTracker ?? throw new ArgumentNullException(nameof(usageTracker));
    }
    /// <summary>
    /// After preparing the appropriate RegiX request you will be able to query it.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <param name="client"></param>
    /// <returns>RegiX response in json format</returns>
    [HttpPost("regix/search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegiXSearchResult))]
    public async Task<IActionResult> RegiXSearchAsync(
        [FromServices] IRequestClient<RegiXSearchCommand> client,
        [FromBody] RegiXSearchRequest request,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("RegiX");

        var logEventCode = LogEventCode.REGIX_SEARCH;
        var eventPayload = BeginAuditLog(logEventCode, request);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetRegiXResponseAsync(client, request, cancellationToken);
        eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.BuildRegiXSource(request));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Gets information for UIC about predefined fieldList.
    /// </summary>
    /// <remarks>
    /// Based on: https://info-regix.egov.bg/public/administrations/-/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualStateV3
    /// </remarks>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="query">UIC parameter is required.</param>
    [HttpGet("tr/getactualstatev3")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegiXSearchResult))]
    public async Task<IActionResult> TrGetActualStateV3Async(
        [FromServices] IRequestClient<RegiXSearchCommand> client,
        [FromQuery] GetActualStateByUICQuery query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Търговски Регистър");
        var logEventCode = LogEventCode.TR_GET_ACTUAL_STATE_V3;
        var eventPayload = BeginAuditLog(logEventCode, query, (nameof(query.UIC), query.UIC));
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }
        var uicParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "UIC",
                new RegiXArgumentParameter()
                {
                    ParameterStringValue = query.UIC,
                    ParameterType = StringParamType
                }
            }
        };

        var defaultFieldLists = "00020,00070,00071,00100,00101,00102,00110,00120,00121,00123,00130,00131,00133,00134,00140,00142,00143,00150,00151,00190,00200,00210,00230,00430,00540,00541,05340,05350,05371,05381,05500,01160"; //0010-Представители; 00110-Начин на представляване; 0054-Обем на представителна власт
        var ParameterStringValue = !string.IsNullOrWhiteSpace(query.AdditionalFieldList) ? string.Join(",", defaultFieldLists, query.AdditionalFieldList) : defaultFieldLists;
        var fieldListParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "FieldList",
                new RegiXArgumentParameter()
                {
                    //FieldList param contains comma separated list of field identifiers
                    ParameterStringValue =  ParameterStringValue,
                    ParameterType = StringParamType
                }
            }
        };

        var request = new RegiXSearchRequest()
        {
            Operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualStateV3",
            Argument = new RegiXArgument()
            {
                Type = "ActualStateRequestV3",
                Xmlns = "http://egov.bg/RegiX/AV/TR/ActualStateRequestV3",
                Parameters = new List<Dictionary<string, RegiXArgumentParameter>>
                {
                    uicParam,
                    fieldListParam
                }
            }
        };
        eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.BuildRegiXSource(request));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetRegiXResponseAsync(client, request, cancellationToken);

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get information about foreign person by LNCh
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="query"></param>
    /// <returns>response.ForeignIdentityInfoResponseType.returnInformations.returnCode: 0000-Success;0100-No data</returns>
    [HttpGet("mvr/getforeignidentityv2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MVRGetForeignIdentityV2Result))]
    [RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
    public async Task<IActionResult> MVRGetForeignIdentityV2Async(
        [FromServices] IRequestClient<MVRGetForeignIdentityV2> client,
        [FromQuery] GetForeignIdentityQuery query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Национален автоматизиран информационен фонд за българските лични документи");
        var logEventCode = LogEventCode.MVR_GET_FOREIGN_IDENTITY_V2;
        var eventPayload = BeginAuditLog(logEventCode, query,
            (AuditLoggingKeys.TargetUid, query.Identifier),
            (AuditLoggingKeys.TargetUidType, query.IdentifierType.ToString()),
            (AuditLogHelper.Source, AuditLogHelper.Naif)
        );
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<MVRGetForeignIdentityV2Result>>(
                new
                {
                    CorrelationId = RequestId,
                    query.Identifier,
                    query.IdentifierType
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get personal identity about citizen
    /// </summary>
    /// <remarks>
    /// Based on: https://info-regix.egov.bg/public/administrations/-/registries/operations/TechnoLogica.RegiX.MVRBDSAdapter.APIService.IMVRBDSAPI/GetPersonalIdentityV2
    /// </remarks>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="query"></param>
    /// <returns>response.PersonalIdentityInfoResponseType.returnInformations.returnCode: 0000-Success;0100-No data</returns>
    [HttpGet("mvr/getpersonalidentityv2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MVRGetPersonalIdentityV2Result))]
    [RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
    public async Task<IActionResult> MVRGetPersonalIdentityV2Async(
        [FromServices] IRequestClient<MVRGetPersonalIdentityV2> client,
        [FromQuery] GetPersonalIdentityQuery query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Национален автоматизиран информационен фонд за българските лични документи");
        var logEventCode = LogEventCode.MVR_GET_PERSONAL_IDENTITY_V2;
        var eventPayload = BeginAuditLog(logEventCode, query,
            (AuditLoggingKeys.TargetUid, query.EGN),
            (AuditLoggingKeys.TargetUidType, IdentifierType.EGN.ToString()),
            (AuditLogHelper.Source, AuditLogHelper.Naif)
        );
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<MVRGetPersonalIdentityV2Result>>(
                new
                {
                    CorrelationId = RequestId,
                    query.EGN,
                    query.IdentityDocumentNumber
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get information about Relatives by EGN
    /// </summary>
    /// <remarks>
    /// Based on: https://info-regix.egov.bg/public/administrations/-/registries/operations/TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI/RelationsSearch
    /// </remarks>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="query"></param>
    /// <returns> 0000-Success;1001-Something went wrong; 0100-No data </returns>
    [HttpGet("grao/relationssearch")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegiXSearchResult))]
    [RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
    public async Task<IActionResult> GRAOGetRelationsAsync(
        [FromServices] IRequestClient<RegiXSearchCommand> client,
        [FromQuery] RelationsSearchRequest query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("ГРАО");
        var logEventCode = LogEventCode.GRAO_RELATIONS_SEARCH;
        var eventPayload = BeginAuditLog(logEventCode, query,
            (AuditLoggingKeys.TargetUid, query.Identifier),
            (AuditLoggingKeys.TargetUidType, IdentifierType.EGN.ToString())
        );
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var regixRequest = new RegiXSearchRequest()
        {
            Operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.RelationsSearch",
            Argument = new RegiXArgument()
            {
                Type = "RelationsRequestType",
                Xmlns = "http://egov.bg/RegiX/GRAO/NBD/RelationsRequest",
                Parameters = new List<Dictionary<string, RegiXArgumentParameter>>
                {
                    new Dictionary<string, RegiXArgumentParameter>
                    {
                        {
                            "EGN",
                            new RegiXArgumentParameter()
                            {
                                ParameterStringValue = query.Identifier,
                                ParameterType = StringParamType
                            }
                        },

                    }
                }
            }
        };
        eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.BuildRegiXSource(regixRequest));

        if (!regixRequest.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var serviceResult = await GetRegiXResponseAsync(client, regixRequest, cancellationToken);

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get information about StateOfPlay by UIC or Case
    /// </summary>
    /// <remarks>
    /// Based on: https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API/GetStateOfPlay
    /// </remarks>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="query"></param>
    /// <returns> 0000-Success;1001-Something went wrong; 0100-No data </returns>
    [HttpGet("bulstat/getstateofplay")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegiXSearchResult))]
    public async Task<IActionResult> BulstatGetStateOfPlayAsync(
        [FromServices] IRequestClient<RegiXSearchCommand> client,
        [FromQuery] GetStateOfPlayRequest query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Регистър Булстат");
        var logEventCode = LogEventCode.BULSTAT_GET_STATE_OF_PLAY;
        var eventPayload = BeginAuditLog(logEventCode, query,
                            ("UIC", query.UIC),
                            ("CaseCode", query.Case?.Court?.Code),
                            ("CaseYear", query.Case?.Year),
                            ("CaseNumber", query.Case?.Number)
        );
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var UICParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "UIC",
                new RegiXArgumentParameter()
                {
                    ParameterStringValue = query.UIC,
                    ParameterType = StringParamType
                }
            },
        };
        var CaseCodeParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "CaseCode",
                new RegiXArgumentParameter()
                {
                    ParameterStringValue = query.Case?.Court?.Code,
                    ParameterType = StringParamType
                }
            },
        };
        var CaseYearParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "CaseYear",
                new RegiXArgumentParameter()
                {
                    ParameterNumberValue = query.Case?.Year,
                    ParameterType = StringParamType
                }
            },
        };
        var CaseNumberParam = new Dictionary<string, RegiXArgumentParameter>
        {
            {
                "CaseNumber",
                new RegiXArgumentParameter()
                {
                    ParameterStringValue = query.Case?.Number,
                    ParameterType = StringParamType
                }
            },
        };

        var regixRequest = new RegiXSearchRequest()
        {
            Operation = "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay",
            Argument = new RegiXArgument()
            {
                Type = "GetStateOfPlayRequest",
                Xmlns = "http://www.bulstat.bg/GetStateOfPlayRequest",
                Parameters = new List<Dictionary<string, RegiXArgumentParameter>>
                {
                    UICParam,
                    CaseCodeParam,
                    CaseYearParam,
                    CaseNumberParam
                },

            }
        };
        eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.BuildRegiXSource(regixRequest));

        if (!regixRequest.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        var serviceResult = await GetRegiXResponseAsync(client, regixRequest, cancellationToken, cacheTimeInMinutes: TimeSpan.FromDays(30).TotalMinutes);

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Reference for the nomenclatures of the BULSTAT register
    /// </summary>
    /// <remarks>
    /// Based on: https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API/FetchNomenclatures
    /// </remarks>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> 0000-Success;1001-Something went wrong; 0100-No data </returns>
    [HttpGet("bulstat/fetchnomenclatures")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegiXSearchResult))]
    public async Task<IActionResult> BulstatFetchNomenclaturesAsync(
        [FromServices] IRequestClient<RegiXSearchCommand> client,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Регистър Булстат");
        var regixRequest = new RegiXSearchRequest()
        {
            Operation = "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.FetchNomenclatures",
            Argument = new RegiXArgument()
            {
                Type = "FetchNomenclatures",
                Xmlns = "http://www.bulstat.bg/StateOfPlayService",

            }
        };

        var serviceResult = await GetRegiXResponseAsync(client, regixRequest, cancellationToken, cacheTimeInMinutes: TimeSpan.FromDays(30).TotalMinutes);

        return Result(serviceResult);
    }

    [HttpGet("otherservices/checkuidrestrictions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckUidRestrictionsResult))]
    [RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
    public async Task<IActionResult> OtherServicesCheckUidRestrictionsAsync(
        [FromServices] IRequestClient<CheckUidRestrictions> client,
        [FromServices] IRequestClient<GetDateOfDeath> dodClient,
        [FromServices] IRequestClient<GetDateOfProhibition> dopClient,
        [FromQuery] CheckUidRestrictionsQuery query,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Национален автоматизиран информационен фонд за българските лични документи");
        var logEventCode = LogEventCode.CHECK_UID_RESTRICTIONS;
        var eventPayload = BeginAuditLog(logEventCode, query,
            (AuditLoggingKeys.TargetUid, query.Uid),
            (AuditLoggingKeys.TargetUidType, query.UidType.ToString())
        );
        if (!query.IsValid())
        {
            return BadRequestWithAuditLog(query, logEventCode, eventPayload);
        }

        if (_configuration.GetValue<bool>("USE_CHECKUIDRESTRICTIONS_FALLBACK"))
        {
            eventPayload["FallbackUsed"] = true;
            eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.DatabaseNaif);
            var dopTask = GetResponseAsync(() =>
                dopClient.GetResponse<ServiceResult<DateOfProhibitionResult>>(
                    new
                    {
                        CorrelationId = RequestId,
                        PersonalId = query.Uid,
                        query.UidType,
                    }, cancellationToken));

            var dodTask = GetResponseAsync(() =>
                dodClient.GetResponse<ServiceResult<DateOfDeathResult>>(
                    new
                    {
                        CorrelationId = RequestId,
                        PersonalId = query.Uid,
                        query.UidType,
                    }, cancellationToken));
            await Task.WhenAll(dopTask, dodTask);
            var dopServiceResult = dopTask.Result;
            var dodServiceResult = dodTask.Result;

            var dateRequestFailed = new System.Net.HttpStatusCode[] { dopServiceResult.StatusCode, dodServiceResult.StatusCode }.Any(s => s != System.Net.HttpStatusCode.OK);
            if (dateRequestFailed)
            {
                var errors = new List<string?>();
                errors.Add($"DOP: {dopServiceResult?.Error}");
                errors.AddRange(dopServiceResult?.Errors?.Select(kp => kp.Value) ?? Array.Empty<string?>());
                errors.Add($"DOD: {dodServiceResult?.Error}");
                errors.AddRange(dodServiceResult?.Errors?.Select(kp => kp.Value) ?? Array.Empty<string?>());

                Logger.LogWarning("Fallback check uid restrictions request failed. Errors: {Errors}", errors.Where(e => !string.IsNullOrWhiteSpace(e)));
                return ResultWithAuditLog(new ServiceResult<CheckUidRestrictionsResult>
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway,
                    Error = "Failed fetching uid restrictions information"
                }, logEventCode, eventPayload);
            }

            DateTime? dateOfProhibition = null;
            if (dopServiceResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dateOfProhibition = dopServiceResult.Result?.Date;
            }
            DateTime? dateOfDeath = null;
            if (dodServiceResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dateOfDeath = dodServiceResult.Result?.Date;
            }

            var commonServiceResult = new ServiceResult<CheckUidRestrictionsResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = new CheckUidRestrictionsResultDTO
                {
                    Response = new CheckUidRestrictionsDataDTO
                    {
                        IsDead = dateOfDeath.HasValue,
                        IsProhibited = dateOfProhibition.HasValue
                    },
                    HasFailed = false
                }
            };
            return ResultWithAuditLog(commonServiceResult, logEventCode, eventPayload);
        }

        eventPayload.Add(AuditLogHelper.Source, AuditLogHelper.Naif);
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<CheckUidRestrictionsResult>>(
                new
                {
                    CorrelationId = RequestId,
                    query.Uid,
                    query.UidType
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    internal class CheckUidRestrictionsResultDTO : CheckUidRestrictionsResult
    {
        public CheckUidRestrictionsState Response { get; set; }
        public virtual bool HasFailed { get; set; } = false;
        public virtual string? Error { get; set; }
    }
    internal class CheckUidRestrictionsDataDTO : CheckUidRestrictionsState

    {
        public bool IsProhibited { get; set; }
        public bool IsDead { get; set; }
        public bool HasRevokedParentalRights { get; set; }
    }

    private async Task<ServiceResult<RegixSearchResultDTO>> GetRegiXResponseAsync(IRequestClient<RegiXSearchCommand> client, RegiXSearchRequest regixRequest, CancellationToken cancellationToken, double cacheTimeInMinutes = 10)
    {
        return await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<RegixSearchResultDTO>>(
                new
                {
                    CorrelationId = RequestId,
                    CacheTimeInMinutes = cacheTimeInMinutes,
                    Command = JsonConvert
                                .SerializeObject(
                                    regixRequest,
                                    new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    })
                }, cancellationToken));
    }
}
