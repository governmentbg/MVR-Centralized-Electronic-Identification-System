using eID.MIS.Contracts.Results;
using eID.MIS.Contracts.SEV.Commands;
using eID.MIS.Contracts.SEV.External;
using eID.MIS.Contracts.SEV.Results;
using eID.MIS.Contracts.SEV.Validators;
using eID.MIS.Service.Database;
using eID.MIS.Service.Entities;
using eID.MIS.Service.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.MIS.Service;

public class EDeliveryRequestsService : BaseService
{
    private readonly ILogger<EDeliveryRequestsService> _logger;
    private readonly DeliveriesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEDeliveryCaller _eDeliveryCaller;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;

    public EDeliveryRequestsService(
        ILogger<EDeliveryRequestsService> logger,
        DeliveriesDbContext context,
        IPublishEndpoint publishEndpoint,
        IEDeliveryCaller eDeliveryCaller,
        IDistributedCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _eDeliveryCaller = eDeliveryCaller ?? throw new ArgumentNullException(nameof(eDeliveryCaller));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ServiceResult<CreatePassiveIndividualProfileResult>> CreatePassiveIndividualProfileAsync(CreatePassiveIndividualProfile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CreatePassiveIndividualProfileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CreatePassiveIndividualProfile), validationResult);
            return BadRequest<CreatePassiveIndividualProfileResult>(validationResult.Errors);
        }

        var eDeliveryResponse = await _eDeliveryCaller.CreatePassiveIndividualProfileAsync(message.Request);
        if (eDeliveryResponse.HasFailed)
        {
            _logger.LogInformation("Request to eDelivery.{MethodName} failed. Error reason: {ErrorReason}", nameof(_eDeliveryCaller.CreatePassiveIndividualProfileAsync), eDeliveryResponse.Error);
            return BadGateway<CreatePassiveIndividualProfileResult>(eDeliveryResponse.Error);
        }

        _logger.LogInformation("Successfully created passive eDelivery profile {ProfileId} for eid {EIdentityId}", eDeliveryResponse.ProfileId, message.EIdentityId);
        CreatePassiveIndividualProfileResult result = new CreatePassiveIndividualProfileResultDTO
        {
            ProfileId = eDeliveryResponse.ProfileId,
        };

        return Ok(result);
    }

    internal class CreatePassiveIndividualProfileResultDTO : CreatePassiveIndividualProfileResult
    {
        public string ProfileId { get; set; }
    }
    public async Task<ServiceResult<SearchUserProfileResult>> SearchUserProfileAsync(SearchUserProfile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SearchUserProfileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SearchUserProfile), validationResult);
            return BadRequest<SearchUserProfileResult>(validationResult.Errors);
        }

        var eDeliveryResponse = await _eDeliveryCaller.SearchUserProfileAsync(new SearchUserProfileQuery { EIdentityId = message.EIdentityId, Identifier = message.Identifier, TargetGroupId = message.TargetGroupId });
        if (eDeliveryResponse.HasFailed)
        {
            _logger.LogWarning("Request to eDelivery.{MethodName} failed. Error reason: {ErrorReason}", nameof(_eDeliveryCaller.SearchUserProfileAsync), eDeliveryResponse.Error);
            return BadGateway<SearchUserProfileResult>(eDeliveryResponse.Error);
        }
        if (eDeliveryResponse.ProfileId is null)
        {
            _logger.LogInformation("Successfully performed search for eid {EIdentityId}. No eDelivery profile.", message.EIdentityId);
            return NotFound<SearchUserProfileResult>(nameof(eDeliveryResponse.ProfileId), message.Identifier);
        }

        _logger.LogInformation("Successfully performed search for eid {EIdentityId} and got eDelivery profile {ProfileId}", message.EIdentityId, eDeliveryResponse.ProfileId);
        SearchUserProfileResult result = new SearchProfileResultDTO
        {
            ProfileId = eDeliveryResponse.ProfileId,
            Identifier = eDeliveryResponse.Identifier,
            Email = eDeliveryResponse.Email,
            Name = eDeliveryResponse.Name,
            Phone = eDeliveryResponse.Phone
        };

        return Ok(result);
    }
    internal class SearchProfileResultDTO : SearchUserProfileResult
    {
        public string ProfileId { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public async Task<ServiceResult<GetProfileResult>> GetUserProfileAsync(GetUserProfile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserProfileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetUserProfile), validationResult);
            return BadRequest<GetProfileResult>(validationResult.Errors);
        }
        var cacheKey = $"eID:MISSEV:{nameof(GetUserProfile)}:{message.ProfileId}";
        GetProfileResult result = new GetProfileResultDTO();
        var cachedResult = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cachedResult))
        {
            result = JsonConvert.DeserializeObject<GetProfileResultDTO>(cachedResult) ?? new GetProfileResultDTO();
        }
        else
        {
            var eDeliveryResponse = await _eDeliveryCaller.GetUserProfileAsync(message.ProfileId);
            if (eDeliveryResponse.HasFailed)
            {
                _logger.LogInformation("Request to eDelivery.{MethodName} failed. Error reason: {ErrorReason}", nameof(_eDeliveryCaller.GetUserProfileAsync), eDeliveryResponse.Error);
                return BadGateway<GetProfileResult>(eDeliveryResponse.Error);
            }
            if (eDeliveryResponse.ProfileId is null)
            {
                _logger.LogWarning("Successfully performed get profile for eid {EIdentityId}. No eDelivery profile found with id {ProfileId}.", message.EIdentityId, message.ProfileId);
                return NotFound<GetProfileResult>(nameof(eDeliveryResponse.ProfileId), message.ProfileId);
            }
            result = new GetProfileResultDTO
            {
                ProfileId = eDeliveryResponse.ProfileId,
                Address = eDeliveryResponse.Address,
                Phone = eDeliveryResponse.Phone,
                Email = eDeliveryResponse.Email,
                Identifier = eDeliveryResponse.Identifier,
                Name = eDeliveryResponse.Name
            };
            await _cache.SetAsync(cacheKey, result, slidingExpireTime: TimeSpan.FromDays(1));
        }

        _logger.LogInformation("Successfully got eDelivery profile {ProfileId} for eid {EIdentityId}", result.ProfileId, message.EIdentityId);
        return Ok(result);
    }
    internal class GetProfileResultDTO : GetProfileResult
    {
        public string ProfileId { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AddressData Address { get; set; }
    }
    public async Task<ServiceResult<SendMessageResult>> SendMessageOnBehalfAsync(SendMessageOnBehalf message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendMessageOnBehalfValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SendMessageOnBehalfAsync), validationResult);
            return BadRequest<SendMessageResult>(validationResult.Errors);
        }

        var eDeliveryResponse = await _eDeliveryCaller.SendMessageOnBehalfAsync(message.Request);
        if (eDeliveryResponse.HasFailed)
        {
            _logger.LogInformation("Request to eDelivery.{MethodName} failed. Error reason: {ErrorReason}", nameof(_eDeliveryCaller.SendMessageOnBehalfAsync), eDeliveryResponse.Error);
            return BadGateway<SendMessageResult>(eDeliveryResponse.Error);
        }

        _logger.LogInformation("Successfully sent eDelivery message {MessageId} to eid {EIdentityId}.", eDeliveryResponse.Result, message.EIdentityId);
        await _context.Deliveries.AddAsync(new DeliveryRequest
        {
            Id = Guid.NewGuid(),
            SystemName = message.SystemName,
            EIdentityId = message.EIdentityId,
            MessageId = eDeliveryResponse.Result,
            ReferencedOrn = message.Request.ReferencedOrn,
            Subject = message.Request.Subject,
            SentOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        SendMessageResult result = new SendMessageResultDTO
        {
            Result = eDeliveryResponse.Result,
        };

        return Ok(result);
    }
    public async Task<ServiceResult<SendMessageResult>> SendMessageAsync(SendMessage message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendMessageValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SendMessageAsync), validationResult);
            return BadRequest<SendMessageResult>(validationResult.Errors);
        }

        var eDeliveryResponse = await _eDeliveryCaller.SendMessageAsync(message.Request);
        if (eDeliveryResponse.HasFailed)
        {
            _logger.LogInformation("Request to eDelivery.{MethodName} failed. Error reason: {ErrorReason}", nameof(_eDeliveryCaller.SendMessageAsync), eDeliveryResponse.Error);
            return BadGateway<SendMessageResult>(eDeliveryResponse.Error);
        }

        _logger.LogInformation("Successfully sent eDelivery message {MessageId} to eid {EIdentityId}.", eDeliveryResponse.Result, message.EIdentityId);
        await _context.Deliveries.AddAsync(new DeliveryRequest
        {
            Id = Guid.NewGuid(),
            SystemName = message.SystemName,
            EIdentityId = message.EIdentityId,
            MessageId = eDeliveryResponse.Result,
            ReferencedOrn = message.Request.ReferencedOrn,
            Subject = message.Request.Subject,
            SentOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        SendMessageResult result = new SendMessageResultDTO
        {
            Result = eDeliveryResponse.Result,
        };

        return Ok(result);
    }
    internal class SendMessageResultDTO : SendMessageResult
    {
        public int Result { get; set; }
    }
    public async Task<ServiceResult<IEnumerable<DeliveryRequestResult>>> GetDeliveriesAsync(GetDeliveries message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDeliveriesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetDeliveriesAsync), validationResult);
            return BadRequest<IEnumerable<DeliveryRequestResult>>(validationResult.Errors);
        }

        var result = await _context.Deliveries
                        .Where(d => d.EIdentityId == message.EIdentityId)
                        .ToListAsync();

        return Ok(result.AsEnumerable<DeliveryRequestResult>());
    }
}
