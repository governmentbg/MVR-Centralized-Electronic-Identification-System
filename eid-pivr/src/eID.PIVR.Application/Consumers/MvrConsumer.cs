using System.Net;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eID.PIVR.Application.Consumers;

public class MvrConsumer : BaseConsumer,
    IConsumer<MVRGetForeignIdentityV2>,
    IConsumer<MVRGetPersonalIdentityV2>,
    IConsumer<CheckUidRestrictions>
{
    private readonly IRegiXService _regixService;
    private readonly INAIFService _naifService;
    private readonly IConfiguration _configuration;

    public MvrConsumer(ILogger<MvrConsumer> logger, INAIFService naifService, IConfiguration configuration) : base(logger)
    {
        // TODO: For future removal.
        _regixService = new RegiXFakeService(new NullLogger<RegiXFakeService>());
        _naifService = naifService ?? throw new ArgumentNullException(nameof(naifService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task Consume(ConsumeContext<MVRGetForeignIdentityV2> context)
    {
        using (Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId))
        {
            Logger.LogInformation("Received message {MessageName}", nameof(MVRGetForeignIdentityV2));
            var useRegix = _configuration.GetValue<bool>("USE_REGIX_FOR_MVR");
            if (useRegix)
            {
                var command = PrepareGetForeignIdentityV2RegixCommand(context);
                Logger.LogInformation("{MessageName} calling RegiX.", nameof(MVRGetForeignIdentityV2));
                var regixServiceResult = await _regixService.SearchAsync(command);

                ServiceResult<MVRGetForeignIdentityV2Result> result = new ServiceResult<MVRGetForeignIdentityV2Result>
                {
                    StatusCode = regixServiceResult.StatusCode,
                    Result = new MVRGetForeignIdentityV2ResultImpl
                    {
                        Response = JsonConvert.DeserializeObject<IDictionary<string, Service.RegiXResponses.ForeignIdentityInfoResponseType>>(JsonConvert.SerializeObject(regixServiceResult?.Result?.Response)) ?? new Dictionary<string, Service.RegiXResponses.ForeignIdentityInfoResponseType>(),
                        HasFailed = regixServiceResult?.Result?.HasFailed ?? true,
                        Error = regixServiceResult?.Result?.Error
                    },
                    Error = regixServiceResult?.Error,
                    Errors = regixServiceResult?.Errors
                };
                Logger.LogInformation("{MessageName} responding.", nameof(MVRGetForeignIdentityV2));
                await RespondAsync(context, result);
            }
            else
            {
                Logger.LogInformation("{MessageName} calling NAIF.", nameof(MVRGetForeignIdentityV2));
                var naifServiceResult = await _naifService.GetForeignIdentityV2Async(context.Message);

                ServiceResult<MVRGetForeignIdentityV2Result> result = new ServiceResult<MVRGetForeignIdentityV2Result>
                {
                    StatusCode = naifServiceResult.StatusCode,
                    Result = new MVRGetForeignIdentityV2ResultImpl
                    {
                        Response = JsonConvert.DeserializeObject<IDictionary<string, Service.RegiXResponses.ForeignIdentityInfoResponseType>>(JsonConvert.SerializeObject(naifServiceResult?.Result?.Response)) ?? new Dictionary<string, Service.RegiXResponses.ForeignIdentityInfoResponseType>(),
                        HasFailed = naifServiceResult?.Result?.HasFailed ?? true,
                        Error = naifServiceResult?.Result?.Error
                    },
                    Error = naifServiceResult?.Error,
                    Errors = naifServiceResult?.Errors
                };
                Logger.LogInformation("{MessageName} responding.", nameof(MVRGetForeignIdentityV2));
                await RespondAsync(context, result);
            }
        }
    }

    public async Task Consume(ConsumeContext<MVRGetPersonalIdentityV2> context)
    {
        using (Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId))
        {
            Logger.LogInformation("Received message {MessageName}", nameof(MVRGetPersonalIdentityV2));
            var useRegix = _configuration.GetValue<bool>("USE_REGIX_FOR_MVR");
            if (useRegix)
            {
                var command = PrepareGetPersonalIdentityV2RegixCommand(context);
                Logger.LogInformation("{MessageName} calling RegiX.", nameof(MVRGetPersonalIdentityV2));
                var regixServiceResult = await _regixService.SearchAsync(command);

                ServiceResult<MVRGetPersonalIdentityV2Result> result = new ServiceResult<MVRGetPersonalIdentityV2Result>
                {
                    StatusCode = regixServiceResult.StatusCode,
                    Result = new MVRGetPersonalIdentityV2ResultImpl
                    {
                        Response = JsonConvert.DeserializeObject<IDictionary<string, Service.RegiXResponses.PersonalIdentityInfoResponseType>>(JsonConvert.SerializeObject(regixServiceResult?.Result?.Response)) ?? new Dictionary<string, Service.RegiXResponses.PersonalIdentityInfoResponseType>(),
                        HasFailed = regixServiceResult?.Result?.HasFailed ?? true,
                        Error = regixServiceResult?.Result?.Error
                    },
                    Error = regixServiceResult?.Error,
                    Errors = regixServiceResult?.Errors
                };
                await RespondAsync(context, result);
            }
            else
            {
                Logger.LogInformation("{MessageName} calling NAIF.", nameof(MVRGetPersonalIdentityV2Result));
                var naifServiceResult = await _naifService.GetPersonalIdentityV2Async(context.Message);

                ServiceResult<MVRGetPersonalIdentityV2Result> result = new ServiceResult<MVRGetPersonalIdentityV2Result>
                {
                    StatusCode = naifServiceResult.StatusCode,
                    Result = new MVRGetPersonalIdentityV2ResultImpl
                    {
                        Response = JsonConvert.DeserializeObject<IDictionary<string, Service.RegiXResponses.PersonalIdentityInfoResponseType>>(JsonConvert.SerializeObject(naifServiceResult?.Result?.Response)) ?? new Dictionary<string, Service.RegiXResponses.PersonalIdentityInfoResponseType>(),
                        HasFailed = naifServiceResult?.Result?.HasFailed ?? true,
                        Error = naifServiceResult?.Result?.Error
                    }, //regixServiceResult.Result, // TODO REMAP
                    Error = naifServiceResult?.Error,
                    Errors = naifServiceResult?.Errors
                };
                Logger.LogInformation("{MessageName} responding.", nameof(MVRGetPersonalIdentityV2Result));
                await RespondAsync(context, result);
            }
        }
    }

    public async Task Consume(ConsumeContext<CheckUidRestrictions> context)
    {
        await ExecuteMethodAsync(context, () => _naifService.CheckUidRestrictionsAsync(context.Message));
    }

    private async Task RespondAsync(ConsumeContext<MVRGetPersonalIdentityV2> context, ServiceResult<MVRGetPersonalIdentityV2Result> result)
    {
        try
        {
            Logger.LogInformation("{MessageName} responding.", nameof(MVRGetPersonalIdentityV2));
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception occurred when execute '{MethodName}'.{MessageName}", nameof(RespondAsync), nameof(MVRGetPersonalIdentityV2));
            await context.RespondAsync(new ServiceResult<MVRGetPersonalIdentityV2Result> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" });
        }
    }

    private async Task RespondAsync(ConsumeContext<MVRGetForeignIdentityV2> context, ServiceResult<MVRGetForeignIdentityV2Result> result)
    {
        try
        {
            Logger.LogInformation("{MessageName} responding.", nameof(MVRGetForeignIdentityV2));
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception occurred when execute '{MethodName}'.{MessageName}", nameof(RespondAsync), nameof(MVRGetForeignIdentityV2));
            await context.RespondAsync(new ServiceResult<MVRGetForeignIdentityV2Result> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" });
        }
    }

    private static RegiXSearchCommandImpl PrepareGetForeignIdentityV2RegixCommand(ConsumeContext<MVRGetForeignIdentityV2> context)
    {
        var parameters = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            {
                "IdentifierType",
                new
                {
                    ParameterStringValue = context.Message.IdentifierType.ToString(),
                    ParameterType = "STRING"
                }
            },

        },
        new Dictionary<string, object>
        {
            {
                "Identifier",
                new
                {
                    ParameterStringValue = context.Message.Identifier,
                    ParameterType = "STRING"
                }
            },
        }
    };

        var regixRequest = new
        {
            Operation = "TechnoLogica.RegiX.MVRERChAdapter.APIService.IMVRERChAPI.GetForeignIdentityV2",
            Argument = new
            {
                Type = "ForeignIdentityInfoRequest",
                Xmlns = "http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest",
                Parameters = parameters
            }
        };
        var command = new RegiXSearchCommandImpl
        {
            CorrelationId = context.Message.CorrelationId,
            Command = JsonConvert
                        .SerializeObject(
                            regixRequest,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                },
                                NullValueHandling = NullValueHandling.Ignore
                            })
        };
        return command;
    }

    private static RegiXSearchCommandImpl PrepareGetPersonalIdentityV2RegixCommand(ConsumeContext<MVRGetPersonalIdentityV2> context)
    {
        var parameters = new List<Dictionary<string, object>>();
        if (!string.IsNullOrWhiteSpace(context.Message.IdentityDocumentNumber))
        {
            parameters.Add(new Dictionary<string, object>
            {
                {
                    "IdentityDocumentNumber",
                    new
                    {
                        ParameterStringValue = context.Message.IdentityDocumentNumber,
                        ParameterType = "STRING"
                    }
                },
            });
        }

        if (!string.IsNullOrWhiteSpace(context.Message.EGN))
        {
            parameters.Add(new Dictionary<string, object>
            {
                {
                    "EGN",
                    new
                    {
                        ParameterStringValue = context.Message.EGN,
                        ParameterType = "STRING"
                    }
                },
            });
        }

        var regixRequest = new
        {
            Operation = "TechnoLogica.RegiX.MVRBDSAdapter.APIService.IMVRBDSAPI.GetPersonalIdentityV2",
            Argument = new
            {
                Type = "PersonalIdentityInfoRequest",
                Xmlns = "http://egov.bg/RegiX/MVR/BDS/PersonalIdentityInfoRequest",
                Parameters = parameters
            }
        };

        var command = new RegiXSearchCommandImpl
        {
            CorrelationId = context.Message.CorrelationId,
            Command = JsonConvert
                        .SerializeObject(
                            regixRequest,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                },
                                NullValueHandling = NullValueHandling.Ignore
                            })
        };
        return command;
    }

    private static object PrepareGetForeignIdentityV2NAIFCommand(ConsumeContext<MVRGetForeignIdentityV2> context)
    {
        // TODO: Prepare the command if needed
        return new { };
    }

    private static object PrepareGetPersonalIdentityV2NAIFCommand(ConsumeContext<MVRGetPersonalIdentityV2> context)
    {
        // TODO: Prepare the command if needed
        return new { };
    }
}

internal class MVRGetPersonalIdentityV2ResultImpl : MVRGetPersonalIdentityV2Result
{
    public dynamic Response { get; set; }
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}
internal class MVRGetForeignIdentityV2ResultImpl : MVRGetForeignIdentityV2Result
{
    public dynamic Response { get; set; }
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}

internal class RegiXSearchCommandImpl : RegiXSearchCommand
{
    public Guid CorrelationId { get; set; }
    public string Command { get; set; } = string.Empty;
    public double CacheTimeInMinutes { get; set; }
}
