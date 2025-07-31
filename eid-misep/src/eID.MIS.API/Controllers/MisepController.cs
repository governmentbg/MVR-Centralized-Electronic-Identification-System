using System.Net;
using eID.MIS.API.Options;
using eID.MIS.API.Responses;
using eID.MIS.Contracts;
using eID.MIS.Contracts.EP.Commands;
using eID.MIS.Contracts.EP.External;
using eID.MIS.Contracts.EP.Results;
using eID.MIS.Contracts.Requests;
using eID.MIS.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace eID.MIS.API.Controllers;

[AllowAnonymous]
//[RoleAuthorization(UserRoles.AllAdmins, allowM2M: true)]
[Route("[controller]/api/v{version:apiVersion}/paymentrequests")]
public class MisepController : BaseV1Controller
{
    private ExchangeRatesOptions _exchangeRates;
    public MisepController(IConfiguration configuration, ILogger<MisepController> logger, AuditLogger auditLogger, IOptions<ExchangeRatesOptions> exchangeRatesOptions) : base(configuration, logger, auditLogger)
    {
        _exchangeRates = exchangeRatesOptions.Value;
        _exchangeRates.Validate();
    }

    /// <summary>
    /// Creates a new payment request in ePayments and stores linking data in our storage.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(Name = nameof(CreatePaymentRequestAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatePaymentRequestResult))]
    public async Task<IActionResult> CreatePaymentRequestAsync(
        [FromServices] IRequestClient<CreatePaymentRequest> client,
        [FromBody] CreatePaymentPayload payload,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.CREATE_PAYMENT_REQUEST;
        var eventPayload = BeginAuditLog(logEventCode, request: payload, targetUserId: null,
                ("CitizenProfileId", payload.CitizenProfileId));

        if (!payload.IsValid())
        {
            return BadRequestWithAuditLog(payload, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<CreatePaymentRequestResult>>(
                new
                {
                    CorrelationId = RequestId,
                    payload.CitizenProfileId,
                    payload.Request,
                    SystemName = GetSystemName()
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Returns the status of the paymentRequestId. IMPORTANT: Makes check in ePayments if the status isn't Paid.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="paymentRequestId"></param>
    /// <param name="citizenProfileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{paymentRequestId}/status", Name = nameof(GetPaymentRequestStatusAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentStatus))]
    public async Task<IActionResult> GetPaymentRequestStatusAsync(
        [FromServices] IRequestClient<GetPaymentRequestStatus> client,
        [FromRoute] Guid paymentRequestId,
        [FromQuery] Guid citizenProfileId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PAYMENT_REQUEST_STATUS;
        var eventPayload = BeginAuditLog(logEventCode, targetUserId: null,
                ("CitizenProfileId", citizenProfileId),
                ("PaymentRequestId", paymentRequestId));

        if (Guid.Empty == paymentRequestId)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(paymentRequestId), $"Invalid parameter {nameof(paymentRequestId)}");
            eventPayload.Add("Reason", $"Invalid parameter {nameof(paymentRequestId)}");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }
        if (Guid.Empty == citizenProfileId)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(citizenProfileId), $"Invalid parameter {nameof(citizenProfileId)}");
            eventPayload.Add("Reason", $"Invalid parameter {nameof(citizenProfileId)}");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<PaymentStatus>>(
                new
                {
                    CorrelationId = RequestId,
                    PaymentRequestId = paymentRequestId,
                    CitizenProfileId = citizenProfileId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    /// <summary>
    /// Returns all payment requests for the given citizenProfileId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="citizenProfileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetPaymentRequestsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PaymentRequestResultResponse>))]
    public async Task<IActionResult> GetPaymentRequestsAsync(
        [FromServices] IRequestClient<GetPaymentRequests> client,
        [FromQuery] Guid citizenProfileId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PAYMENT_REQUESTS;
        var eventPayload = BeginAuditLog(logEventCode,
                ("CitizenProfileId", citizenProfileId));

        if (Guid.Empty == citizenProfileId)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(citizenProfileId), $"Invalid parameter {nameof(citizenProfileId)}");
            eventPayload.Add("Reason", $"Invalid parameter {nameof(citizenProfileId)}");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<PaymentRequestResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    CitizenProfileId = citizenProfileId
                }, cancellationToken));

        if (serviceResult is null)
        {
            return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
        }

        var result = new ServiceResult<IEnumerable<PaymentRequestResultResponse>>
        {
            Result = serviceResult.Result?.Select(ProjectToDto),
            StatusCode = serviceResult.StatusCode,
            Error = serviceResult.Error,
            Errors = serviceResult.Errors,
        };
        return ResultWithAuditLog(result, logEventCode, eventPayload);
    }

    /// <summary>
    /// Returns all clients in ePayments by eik
    /// </summary>
    /// <param name="client"></param>
    /// <param name="eik"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("~/[controller]/api/v{version:apiVersion}/ePayments/get-clients-by-eik", Name = nameof(GetClientsByEikAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetClientsByEikResult))]
    public async Task<IActionResult> GetClientsByEikAsync(
        [FromServices] IRequestClient<GetClientsByEik> client,
        [FromQuery] string eik,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_CLIENTS_BY_EIK;
        var eventPayload = BeginAuditLog(logEventCode, new { Eik = eik });
        if (string.IsNullOrWhiteSpace(eik))
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(eik), $"Invalid parameter {nameof(eik)}");
            eventPayload.Add("Reason", $"Invalid parameter {nameof(eik)}");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<GetClientsByEikResult>>(
                new
                {
                    CorrelationId = RequestId,
                    Eik = eik
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    private PaymentRequestResultResponse ProjectToDto(PaymentRequestResult item)
    {
        decimal amountBGN = 0;
        decimal amountEUR = 0;

        if (item.Currency == "BGN")
        {
            amountBGN = item.Amount;
            amountEUR = Math.Round(item.Amount / _exchangeRates.BGN_TO_EUR, 2, MidpointRounding.AwayFromZero);
        }
        else if (item.Currency == "EUR")
        {
            amountEUR = item.Amount;
            amountBGN = Math.Round(item.Amount * _exchangeRates.EUR_TO_BGN, 2, MidpointRounding.AwayFromZero);
        }

        return new PaymentRequestResultResponse
        {
            EPaymentId = item.EPaymentId,
            CitizenProfileId = item.CitizenProfileId,
            CreatedOn = item.CreatedOn,
            PaymentDeadline = item.PaymentDeadline,
            PaymentDate = item.PaymentDate,
            Status = item.Status,
            AccessCode = item.AccessCode,
            RegistrationTime = item.RegistrationTime,
            ReferenceNumber = item.ReferenceNumber,
            Reason = item.Reason,
            Currency = item.Currency,
            Amount = item.Amount,
            LastSync = item.LastSync,
            AmountBGN = amountBGN,
            AmountEUR = amountEUR
        };
    }
}
