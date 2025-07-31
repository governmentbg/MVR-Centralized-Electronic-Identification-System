using eID.PUN.Contracts.Commands;
using eID.PUN.Contracts.Results;
using eID.PUN.Service.Database;
using eID.PUN.Service.Entities;
using eID.PUN.Service.Validators;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eID.PUN.Service;

public class CarriersService : BaseService
{
    private readonly ILogger<CarriersService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CarriersService(
        ILogger<CarriersService> logger,
        IPublishEndpoint publishEndpoint,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<Guid>> RegisterAsync(RegisterCarrier message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterCarrierValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        var carrier = new Carrier
        {
            Id = Guid.NewGuid(),
            ModifiedOn = DateTime.UtcNow,
            SerialNumber = message.SerialNumber,
            Type = message.Type,
            CertificateId = message.CertificateId,
            EId = message.EId
        };

        await _context.Carriers.AddAsync(carrier);

        // Execute query
        await _context.SaveChangesAsync();
        _logger.LogInformation("New carrier is registered: {SerialNumber}", carrier.SerialNumber);

        await _publishEndpoint.Publish<NotifyEIds>(new
        {
            message.CorrelationId,
            EIds = new List<Guid> { carrier.EId },
            EventCode = Events.CarrierCreated.Code,
            Events.CarrierCreated.Translations
        });

        // Result
        return Ok(carrier.Id);
    }

    public async Task<ServiceResult<IEnumerable<CarrierResult>>> GetByFilterAsync(GetCarriersByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetCarriersByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<CarrierResult>>(validationResult.Errors);
        }

        // Action
        var query = _context.Carriers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(message.SerialNumber))
        {
            query = query.Where(c => c.SerialNumber == message.SerialNumber);
        }
        if (message.EId != Guid.Empty)
        {
            query = query.Where(c => c.EId == message.EId);
        }
        if (message.CertificateId != Guid.Empty)
        {
            query = query.Where(c => c.CertificateId == message.CertificateId);
        }
        if (!string.IsNullOrWhiteSpace(message.Type))
        {
            query = query.Where(d => d.Type == message.Type);
        }
        var result = await query.ToListAsync();

        // We send notification to carriers' owners. If we do find any Carriers
        if (result.Any())
        {
            var distinctEIds = result.Select(c => c.EId).ToHashSet();

            await _publishEndpoint.Publish<NotifyEIds>(new
            {
                message.CorrelationId,
                EIds = distinctEIds,
                EventCode = Events.CarrierListAccessed.Code,
                Events.CarrierListAccessed.Translations
            });
        }

        // Result
        return Ok(result.AsEnumerable<CarrierResult>());
    }
}
