using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.PAN.Service;

public class UserNotificationsService : BaseService
{
    private readonly ILogger<UserNotificationsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;

    public UserNotificationsService(
        ILogger<UserNotificationsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<IPaginatedData<RegisteredSystemResult>>> GetByFilterAsync(GetSystemsAndNotificationsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserNotificationsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<RegisteredSystemResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var registeredSystems = _context.RegisteredSystems
            .Include(rs => rs.Events)
            .AsQueryable()
            .Where(s => s.IsApproved && s.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(message.SystemName))
        {
            registeredSystems = registeredSystems
                .Where(s => s.Name.Contains(message.SystemName));
        }

        // Pagination
        // Execute query
        var result = await PaginatedData<RegisteredSystemResult>.CreateAsync(registeredSystems.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IPaginatedData<Guid>>> GetDeactivatedAsync(GetDeactivatedUserNotifications message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDeactivatedUserNotificationsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<Guid>>(validationResult.Errors);
        }

        // Action
        var query = _context.DeactivatedUserEvents
            .Where(d => d.UserId == message.UserId)
            .Select(d => d.SystemEventId)
            .OrderBy(d => d);

        // Pagination
        // Execute query
        var result = await PaginatedData<Guid>.CreateAsync(query, message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult> RegisterDeactivatedEventsAsync(RegisterDeactivatedEvents message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterDeactivatedEventsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Action
        var currentEvents = await _context.DeactivatedUserEvents
            .Where(de => de.UserId == message.UserId)
            .ToListAsync() ?? new List<DeactivatedUserEvent>();

        // Delete all records
        if (!message.Ids.Any())
        {
            if (currentEvents.Any())
            {
                _context.RemoveRange(currentEvents);
                await _context.SaveChangesAsync();
            }

            // Result
            return NoContent();
        }

        // Check if all received ids are valid
        var availableIds = await _context.SystemEvents
            .Include(se => se.RegisteredSystem)
            .Where(se => se.RegisteredSystem.IsApproved)
            .Where(se => message.Ids.Contains(se.Id))
            .Select(se => se.Id)
            .ToArrayAsync();

        var missingIds = message.Ids.Except(availableIds);

        if (missingIds.Any())
        {
            return BadRequest(nameof(message.Ids), $"{string.Join(',', missingIds.Select(d => d.ToString()))} are invalid");
        }

        // DeactivatedEvents are empty
        if (!currentEvents.Any())
        {
            _context.DeactivatedUserEvents.AddRange(
                message.Ids.Select(id => CreateFromMessage(message, id)));
        }
        else
        {
            // Process update
            currentEvents.ToList()
                .ForEach(ce =>
                {
                    if (message.Ids.Contains(ce.Id))
                    {
                        ce.ModifiedBy = message.ModifiedBy;
                        ce.ModifiedOn = DateTime.UtcNow;
                    }
                });

            // Process deleted
            var deletedEvents = currentEvents
                .Where(ce => !message.Ids.Contains(ce.SystemEventId));
            if (deletedEvents.Any())
            {
                _context.DeactivatedUserEvents
                    .RemoveRange(deletedEvents);
            }

            // Process new events
            var newEvents = message.Ids
                .Where(mIds => !currentEvents.Any(ce => mIds == ce.SystemEventId));
            if (newEvents.Any())
            {
                _context.DeactivatedUserEvents
                    .AddRange(newEvents
                        .Select(ne => CreateFromMessage(message, ne)));
            }
        }

        // Execute query
        await _context.SaveChangesAsync();

        // Result
        return NoContent();
    }

    private static DeactivatedUserEvent CreateFromMessage(RegisterDeactivatedEvents message, Guid id)
    {
        return new DeactivatedUserEvent
        {
            SystemEventId = id,
            UserId = message.UserId,
            ModifiedBy = message.ModifiedBy,
            ModifiedOn = DateTime.UtcNow
        };
    }
}
