using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.PAN.Service;

public class UserNotificationChannelsService : BaseService
{
    private readonly ILogger<UserNotificationChannelsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;

    public UserNotificationChannelsService(
        ILogger<UserNotificationChannelsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<IPaginatedData<UserNotificationChannelResult>>> GetByFilterAsync(GetUserNotificationChannelsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserNotificationChannelsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<UserNotificationChannelResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var approvedRegisteredSystemIds = _context.RegisteredSystems.Where(rs => rs.IsApproved).Select(rs => rs.Id).ToList();
        var notificationChannels = _context.NotificationChannels
            .Where(nc => nc.IsBuiltIn || approvedRegisteredSystemIds.Contains(nc.SystemId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(message.ChannelName))
        {
            notificationChannels = notificationChannels
                .Where(s => s.Name.Contains(message.ChannelName));
        }

        // Pagination
        // Execute query
        var result = await PaginatedData<UserNotificationChannelResult>.CreateAsync(notificationChannels.OrderBy(d => d.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IPaginatedData<Guid>>> GetSelectedAsync(GetUserNotificationChannels message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserNotificationChannelsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<Guid>>(validationResult.Errors);
        }

        // Action
        var query = _context.UserNotificationChannels
            .Where(d => d.UserId == message.UserId)
            .Select(d => d.NotificationChannelId);

        // Pagination
        // Execute query
        var result = await PaginatedData<Guid>.CreateAsync(query.OrderBy(d => d), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult> RegisterSelectedAsync(RegisterUserNotificationChannels message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterUserNotificationChannelsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Action
        var smtpChannel = await _context.NotificationChannels.FirstOrDefaultAsync(nc => nc.Name == ConfigurationsConstants.SMTP);
        if (smtpChannel is null)
        {
            return InternalServerError($"Notification channel with name {ConfigurationsConstants.SMTP} is not found in the system");
        }

        var channelIds = message.Ids.ToHashSet();
        // Add permanent SMTP notification channel
        channelIds.Add(smtpChannel.Id);

        var currentChannels = await _context.UserNotificationChannels
            .Where(uc => uc.UserId == message.UserId)
            .ToListAsync() ?? new List<UserNotificationChannel>();

        // Check if all received ids are valid
        var availableIds = await _context.NotificationChannels
            .Where(se => channelIds.Contains(se.Id))
            .Select(se => se.Id)
            .ToArrayAsync();

        var missingIds = channelIds.Except(availableIds);
        if (missingIds.Any())
        {
            return BadRequest(nameof(message.Ids), $"{string.Join(',', missingIds.Select(d => d.ToString()))} are invalid");
        }

        // Process update
        currentChannels.ToList()
            .ForEach(ce =>
            {
                if (channelIds.Contains(ce.Id))
                {
                    ce.ModifiedBy = message.ModifiedBy;
                    ce.ModifiedOn = DateTime.UtcNow;
                }
            });

        // Process deleted
        var deletedEvents = currentChannels
            .Where(ce => !channelIds.Contains(ce.NotificationChannelId));
        if (deletedEvents.Any())
        {
            _context.UserNotificationChannels
                .RemoveRange(deletedEvents);
        }

        // Process new events
        var newEvents = channelIds
            .Where(mIds => !currentChannels.Any(ce => mIds == ce.NotificationChannelId));
        if (newEvents.Any())
        {
            _context.UserNotificationChannels
                .AddRange(newEvents
                    .Select(ne => CreateFromMessage(message, ne)));
        }

        // Execute query
        await _context.SaveChangesAsync();

        // Result
        return NoContent();
    }

    private static UserNotificationChannel CreateFromMessage(RegisterUserNotificationChannels message, Guid id)
    {
        return new UserNotificationChannel
        {
            NotificationChannelId = id,
            UserId = message.UserId,
            ModifiedBy = message.ModifiedBy,
            ModifiedOn = DateTime.UtcNow
        };
    }
}
