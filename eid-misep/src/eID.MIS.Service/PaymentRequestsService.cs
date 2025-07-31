using eID.MIS.Contracts.EP.Commands;
using eID.MIS.Contracts.EP.Enums;
using eID.MIS.Contracts.EP.Results;
using eID.MIS.Contracts.Results;
using eID.MIS.Service.Database;
using eID.MIS.Service.Entities;
using eID.MIS.Service.Validators;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eID.MIS.Service;

public class PaymentRequestsService : BaseService
{
    private readonly ILogger<PaymentRequestsService> _logger;
    private readonly PaymentsDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEPaymentsCaller _ePaymentsCaller;
    private readonly IConfiguration _configuration;

    public PaymentRequestsService(
        ILogger<PaymentRequestsService> logger,
        PaymentsDbContext context,
        IPublishEndpoint publishEndpoint,
        IEPaymentsCaller ePaymentsCaller,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _ePaymentsCaller = ePaymentsCaller ?? throw new ArgumentNullException(nameof(ePaymentsCaller));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<ServiceResult<CreatePaymentRequestResult>> CreatePaymentRequestAsync(CreatePaymentRequest message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CreatePaymentRequestValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CreatePaymentRequest), validationResult);
            return BadRequest<CreatePaymentRequestResult>(validationResult.Errors);
        }

        var referenceNumberAlreadyExists = await _context.PaymentRequests.AnyAsync(pr => pr.ReferenceNumber == message.Request.PaymentData.ReferenceNumber);
        if (referenceNumberAlreadyExists)
        {
            _logger.LogWarning("Tried creating payment request with reference number that already exists. {ReferenceNumber}", message.Request.PaymentData.ReferenceNumber);
            return Conflict<CreatePaymentRequestResult>(nameof(PaymentRequest.ReferenceNumber), message.Request.PaymentData.ReferenceNumber);
        }


        var paymentId = Guid.NewGuid();
        var dateTimeNow = DateTime.UtcNow;
        var deadlineLimit = _configuration.GetValue<int>("PaymentDeadlineLimitInDays", PaymentRequest.PaymentDeadlineLimitInDays);
        var paymentDeadline = dateTimeNow.Add(TimeSpan.FromDays(deadlineLimit));
        var sofiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia");
        var dateTimeSofia = TimeZoneInfo.ConvertTimeFromUtc(paymentDeadline, sofiaTimeZone);
        message.Request.PaymentData.ExpirationDate = dateTimeSofia.ToString("s");
        message.Request.PaymentData.TypeCode = 0; // Такси за административни услуги | 448007
        message.Request.PaymentData.PaymentId = paymentId.ToString();
        var ePaymentsResponse = await _ePaymentsCaller.CreatePaymentAsync(message.Request);
        if (ePaymentsResponse is null)
        {
            _logger.LogInformation("Null response from ePayments.");
            return BadGateway<CreatePaymentRequestResult>("No response from ePayments.");
        }
        if (ePaymentsResponse.HasFailed)
        {
            _logger.LogInformation("Request to ePayments failed. Error reason: {ErrorReason}", ePaymentsResponse.Error);
            return BadGateway<CreatePaymentRequestResult>(ePaymentsResponse.Error);
        }

        var newEntity = new PaymentRequest
        {
            Id = paymentId,
            CitizenProfileId = message.CitizenProfileId,
            EPaymentId = ePaymentsResponse.PaymentId,
            CreatedOn = dateTimeNow,
            PaymentDeadline = paymentDeadline,
            AccessCode = ePaymentsResponse.AccessCode,
            RegistrationTime = DateTimeOffset.FromUnixTimeSeconds(ePaymentsResponse.RegistrationTime / 1000).UtcDateTime,
            Status = PaymentStatus.Pending,
            InitiatorSystemName = message.SystemName,
            ReferenceNumber = message.Request.PaymentData.ReferenceNumber,
            Reason = message.Request.PaymentData.Reason,
            Amount = message.Request.PaymentData.Amount,
            Currency = message.Request.PaymentData.Currency,
            LastSync = dateTimeNow
        };
        try
        {
            await _context.PaymentRequests.AddAsync(newEntity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created payment request {PaymentRequestId}", newEntity.Id);
            CreatePaymentRequestResult result = new CreatePaymentRequestResultDTO
            {
                Id = newEntity.Id,
                PaymentDeadline = newEntity.PaymentDeadline,
                AccessCode = newEntity.AccessCode
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed creating payment request");
            await _publishEndpoint.Publish<SuspendPaymentRequestInEPayments>(new
            {
                PaymentRequestId = ePaymentsResponse.PaymentId
            });
            return InternalServerError<CreatePaymentRequestResult>("Failed persisting payment request.");
        }
    }

    internal class CreatePaymentRequestResultDTO : CreatePaymentRequestResult
    {
        public Guid Id { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public string AccessCode { get; set; }
    }

    public async Task<ServiceResult> SuspendPaymentRequestInEPaymentsAsync(SuspendPaymentRequestInEPayments message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SuspendPaymentRequestInEPaymentsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SuspendPaymentRequestInEPayments), validationResult);
            return BadRequest(validationResult.Errors);
        }
        var ePaymentsResponse = await _ePaymentsCaller.SuspendPaymentAsync(message.PaymentRequestId);
        if (ePaymentsResponse is null)
        {
            _logger.LogInformation("Null response from ePayments.");
            return BadGateway("No response from ePayments.");
        }
        if (ePaymentsResponse.HasFailed)
        {
            _logger.LogInformation("Request to ePayments failed. Error reason: {ErrorReason}", ePaymentsResponse.Error);
            return BadGateway(ePaymentsResponse.Error);
        }
        return Ok();
    }

    public async Task<ServiceResult<PaymentStatus>> GetPaymentRequestStatusAsync(GetPaymentRequestStatus message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetPaymentRequestStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetPaymentRequestStatus), validationResult);
            return BadRequest<PaymentStatus>(validationResult.Errors);
        }
        var dbEntity = await _context.PaymentRequests.FirstOrDefaultAsync(pr => pr.Id == message.PaymentRequestId && pr.CitizenProfileId == message.CitizenProfileId);
        if (dbEntity is null)
        {
            return NotFound<PaymentStatus>(nameof(PaymentRequest.Id), message.PaymentRequestId);
        }

        if (dbEntity.Status == PaymentStatus.Paid || dbEntity.Status == PaymentStatus.TimedOut)
        {
            return Ok(dbEntity.Status);
        }

        var ePaymentsResponse = await _ePaymentsCaller.GetPaymentStatusAsync(dbEntity.EPaymentId);
        if (ePaymentsResponse is null)
        {
            _logger.LogInformation("Null response from ePayments.");
            return BadGateway<PaymentStatus>("No response from ePayments.");
        }
        if (ePaymentsResponse.HasFailed)
        {
            _logger.LogInformation("Request to ePayments failed. Error reason: {ErrorReason}", ePaymentsResponse.Error);
            return BadGateway<PaymentStatus>(ePaymentsResponse.Error);
        }

        try
        {
            if (ePaymentsResponse.Status == PaymentStatusType.Paid
                || ePaymentsResponse.Status == PaymentStatusType.Expired)
            {
                var oldStatus = dbEntity.Status;
                dbEntity.Status = ePaymentsResponse.Status switch
                {
                    PaymentStatusType.Paid => PaymentStatus.Paid,
                    PaymentStatusType.Expired => PaymentStatus.TimedOut,
                    _ => dbEntity.Status
                };
                dbEntity.PaymentDate = DateTimeOffset.FromUnixTimeSeconds(ePaymentsResponse.ChangeTime / 1000).UtcDateTime;
                _logger.LogInformation("Payment {PaymentId} status will be updated from {OldStatus} to {NewStatus}", dbEntity.Id, oldStatus, dbEntity.Status);
            }
            dbEntity.LastSync = DateTime.UtcNow;
            _context.SaveChanges();
            _logger.LogInformation("Payment {PaymentId} last sync updated.", dbEntity.Id);
            return Ok(dbEntity.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed updating payment request {PaymentRequestId} status. ePayments status: {ePaymentsStatus}", dbEntity.Id, ePaymentsResponse.Status);
            return InternalServerError<PaymentStatus>("Failed updating payment request.");
        }
    }

    public async Task<ServiceResult<IEnumerable<PaymentRequestResult>>> GetPaymentRequestsAsync(GetPaymentRequests message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetPaymentRequestsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetPaymentRequests), validationResult);
            return BadRequest<IEnumerable<PaymentRequestResult>>(validationResult.Errors);
        }
        var dbResult = await _context.PaymentRequests.Where(r => r.CitizenProfileId == message.CitizenProfileId).ToListAsync();
        return Ok(dbResult.AsEnumerable<PaymentRequestResult>());
    }

    public async Task<ServiceResult<GetClientsByEikResult>> GetClientsByEikAsync(GetClientsByEik message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetClientsByEikValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetClientsByEikResult), validationResult);
            return BadRequest<GetClientsByEikResult>(validationResult.Errors);
        }

        var ePaymentsResponse = await _ePaymentsCaller.GetClientsByEikAsync(message.Eik);
        if (ePaymentsResponse is null)
        {
            _logger.LogWarning("Null response from {CommandName} ePayments.", nameof(GetClientsByEikResult));
            return BadGateway<GetClientsByEikResult>("No response from ePayments.");
        }
        if (ePaymentsResponse.HasFailed)
        {
            _logger.LogWarning("Request to ePayments {CommandName} failed. Error reason: {ErrorReason}", nameof(GetClientsByEikResult), ePaymentsResponse.Error);
            return BadGateway<GetClientsByEikResult>(ePaymentsResponse.Error);
        }

        return Ok(ePaymentsResponse.Data);
    }
}
