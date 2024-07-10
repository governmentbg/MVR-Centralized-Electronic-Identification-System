using System.Net;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Events;
using eID.PAN.Contracts.Results;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Validators;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.PAN.Service;

public class NotificationChannelsService : BaseService
{
    private readonly ILogger<NotificationChannelsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public NotificationChannelsService(ILogger<NotificationChannelsService> logger, IDistributedCache cache, ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ServiceResult<INotificationChannelsData<NotificationChannelResult>>> GetAllChannelsAsync()
    {
        // Prepare query
        var pendingNotificationChannels = _context.NotificationChannelsPending
            .AsQueryable();

        var approvedNotificationChannels = _context.NotificationChannels
            .AsQueryable();

        var rejectedNotificationChannels = _context.NotificationChannelsRejected
            .AsQueryable();

        var archivedNotificationChannels = _context.NotificationChannelsArchive
            .AsQueryable();

        // Execute query
        var result = await NotificationChannelsData<NotificationChannelResult>.CreateAsync(pendingNotificationChannels, approvedNotificationChannels, rejectedNotificationChannels, archivedNotificationChannels);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<Guid>> RegisterChannelAsync(RegisterNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterNotificationChannelValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Not valid NotificationChannel message. Validation failed with errors: {Errors}.", validationResult.Errors);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var approvedChannelWithSameName = await _context.NotificationChannels
            .FirstOrDefaultAsync(nc => nc.Name.ToLower() == message.Name.ToLower());

        if (approvedChannelWithSameName != null)
        {
            _logger.LogInformation("NotificationChannel with this name already exist : {Name}.", message.Name);
            return Conflict<Guid>(nameof(message.Name), message.Name);
        }

        var pendingChannelWithSameName = await _context.NotificationChannelsPending
            .FirstOrDefaultAsync(nc => nc.Name.ToLower() == message.Name.ToLower());

        if (pendingChannelWithSameName != null &&
            pendingChannelWithSameName.SystemId != message.SystemId)
        {
            _logger.LogInformation("Pending notification channel with this name already exist : {Name}.", message.Name);
            return Conflict<Guid>(nameof(message.Name), message.Name);
        }

        // Action
        //update channel with new version from message
        if (pendingChannelWithSameName != null &&
            pendingChannelWithSameName.SystemId == message.SystemId)
        {
            pendingChannelWithSameName.ModifiedBy = message.ModifiedBy;
            pendingChannelWithSameName.ModifiedOn = DateTime.UtcNow;
            pendingChannelWithSameName.Name = message.Name.Trim();
            pendingChannelWithSameName.Description = message.Description;
            //pendingChannelWithSameName.SystemId = message.SystemId; //not sure about this field
            pendingChannelWithSameName.CallbackUrl = message.CallbackUrl;
            pendingChannelWithSameName.Price = message.Price;
            pendingChannelWithSameName.InfoUrl = message.InfoUrl;

            message.Translations.ToList()
                .ForEach(t => pendingChannelWithSameName.Translations.Add(CreateFromMessageNCTranslation(t)));

            _context.Entry(pendingChannelWithSameName);

            // Execute query
            await _context.SaveChangesAsync();
            _logger.LogInformation("Pending notification channel updated: {Name}", pendingChannelWithSameName.Name);

            // Result
            return Ok(pendingChannelWithSameName.Id);
        }
        else //insert new channel
        {
            var notificationChannel = new NotificationChannelPending
            {
                Id = Guid.NewGuid(),
                ModifiedBy = message.ModifiedBy,
                ModifiedOn = DateTime.UtcNow,
                Name = message.Name.Trim(),
                Description = message.Description,
                SystemId = message.SystemId,
                CallbackUrl = message.CallbackUrl,
                Price = message.Price,
                InfoUrl = message.InfoUrl
            };

            message.Translations.ToList()
                .ForEach(t => notificationChannel.Translations.Add(CreateFromMessageNCTranslation(t)));

            await _context.NotificationChannelsPending.AddAsync(notificationChannel);

            // Execute query
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification channel registered in pending state: {Name}", notificationChannel.Name);

            // Result
            return Ok(notificationChannel.Id);
        }
    }

    public async Task<ServiceResult<Guid>> ModifyChannelAsync(ModifyNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ModifyNotificationChannelValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Not valid NotificationChannel message. Validation failed with errors: {Errors}.", validationResult.Errors);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var pendingChannelWithSameName = await _context.NotificationChannelsPending
            .FirstOrDefaultAsync(nc => nc.Name.ToLower() == message.Name.ToLower());

        if (pendingChannelWithSameName != null &&
            pendingChannelWithSameName.SystemId != message.SystemId)
        {
            _logger.LogInformation("Pending notification channel with this name already exist : {Name}.", message.Name);
            return Conflict<Guid>(nameof(message.Name), message.Name);
        }

        var notificationChannel = await _context.NotificationChannels.FindAsync(message.Id);
        if (notificationChannel is null)
        {
            _logger.LogInformation("NotificationChannel with this Id does not exist : {Id}.", message.Id);
            return NotFound<Guid>(nameof(message.Id), message.Id);
        }

        // Action
        //update channel with new version from message
        if (pendingChannelWithSameName != null &&
            pendingChannelWithSameName.SystemId == message.SystemId)
        {
            pendingChannelWithSameName.ModifiedBy = message.ModifiedBy;
            pendingChannelWithSameName.ModifiedOn = DateTime.UtcNow;
            pendingChannelWithSameName.Name = message.Name.Trim();
            pendingChannelWithSameName.Description = message.Description;
            //pendingChannelWithSameName.SystemId = message.SystemId; //not sure about this field
            pendingChannelWithSameName.CallbackUrl = message.CallbackUrl;
            pendingChannelWithSameName.Price = message.Price;
            pendingChannelWithSameName.InfoUrl = message.InfoUrl;

            message.Translations.ToList()
                .ForEach(t => pendingChannelWithSameName.Translations.Add(CreateFromMessageNCTranslation(t)));

            _context.Entry(pendingChannelWithSameName);

            // Execute query
            await _context.SaveChangesAsync();
            _logger.LogInformation("Pending notification channel updated: {Name}", pendingChannelWithSameName.Name);

            // Result
            return Ok(pendingChannelWithSameName.Id);
        }
        else
        {
            var modifiedNotificationChannel = new NotificationChannelPending
            {
                Id = Guid.NewGuid(),
                ModifiedBy = message.ModifiedBy,
                ModifiedOn = DateTime.UtcNow,
                Name = message.Name.Trim(),
                Description = message.Description,
                SystemId = message.SystemId,
                CallbackUrl = message.CallbackUrl,
                Price = message.Price,
                InfoUrl = message.InfoUrl
            };

            message.Translations.ToList()
                .ForEach(t => modifiedNotificationChannel.Translations.Add(CreateFromMessageNCTranslation(t)));

            await _context.NotificationChannelsPending.AddAsync(modifiedNotificationChannel);

            // Execute query
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification channel registered in pending state: {Name}", modifiedNotificationChannel.Name);

            // Result
            return Ok(modifiedNotificationChannel.Id);
        }
    }

    public async Task<ServiceResult<Guid>> ApproveChannelAsync(ApproveNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var notificationChannel = await _context.NotificationChannelsPending.FindAsync(message.Id);
        if (notificationChannel is null)
        {
            _logger.LogInformation("NotificationChannel with this Id does not exist : {Id}.", message.Id);
            return NotFound<Guid>(nameof(message.Id), message.Id);
        }

        var approvedNotificationChannel = await _context.NotificationChannels.FirstOrDefaultAsync(nc => !nc.IsBuiltIn && nc.Name.ToLower() == notificationChannel.Name.ToLower());

        if (approvedNotificationChannel == null)
        {
            approvedNotificationChannel = new NotificationChannelApproved
            {
                Id = Guid.NewGuid(), //There is no approved channel with this name yet. It's safe to create a new id
                ModifiedBy = message.ModifiedBy,
                ModifiedOn = DateTime.UtcNow,
                Name = notificationChannel.Name.Trim(),
                Description = notificationChannel.Description,
                SystemId = notificationChannel.SystemId,
                CallbackUrl = notificationChannel.CallbackUrl,
                Price = notificationChannel.Price,
                InfoUrl = notificationChannel.InfoUrl,
                Translations = notificationChannel.Translations,
                IsBuiltIn = notificationChannel.IsBuiltIn
            };

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var entry = _context.NotificationChannelsPending.Remove(notificationChannel);
                    var approvedEntity = await _context.NotificationChannels.AddAsync(approvedNotificationChannel);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Approve notification channel {Name}: Saving to DB failed.", notificationChannel.Name);
                    transaction.Rollback();
                    return new ServiceResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Error = "Something went wrong." };
                }
            }
        }
        else
        {
            // Applying changes on already existing APPROVED channel,
            // DO NOT reset the Id.
            approvedNotificationChannel.ModifiedBy = message.ModifiedBy;
            approvedNotificationChannel.ModifiedOn = DateTime.UtcNow;
            approvedNotificationChannel.Name = notificationChannel.Name.Trim();
            approvedNotificationChannel.Description = notificationChannel.Description;
            approvedNotificationChannel.SystemId = notificationChannel.SystemId;
            approvedNotificationChannel.CallbackUrl = notificationChannel.CallbackUrl;
            approvedNotificationChannel.Price = notificationChannel.Price;
            approvedNotificationChannel.InfoUrl = notificationChannel.InfoUrl;
            approvedNotificationChannel.Translations = notificationChannel.Translations;
            _context.Entry(approvedNotificationChannel);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.NotificationChannelsPending.Remove(notificationChannel);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Approve notification channel {Name}: Saving to DB failed.", notificationChannel.Name);
                    transaction.Rollback();
                    return new ServiceResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Error = "Something went wrong." };
                }
            }
        }

        _logger.LogInformation("Notification channel approved: {Name}", notificationChannel.Name);
        // Result
        return Ok(approvedNotificationChannel.Id);
    }

    public async Task<ServiceResult<Guid>> RejectChannelAsync(RejectNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var notificationChannel = await _context.NotificationChannelsPending.FindAsync(message.Id);
        if (notificationChannel is null)
        {
            _logger.LogInformation("NotificationChannel with this Id does not exist : {Id}.", message.Id);
            return NotFound<Guid>(nameof(message.Id), message.Id);
        }

        var rejectedNotificationChannel = new NotificationChannelRejected
        {
            Id = Guid.NewGuid(),
            ModifiedBy = message.ModifiedBy,
            ModifiedOn = DateTime.UtcNow,
            Name = notificationChannel.Name.Trim(),
            Description = notificationChannel.Description,
            SystemId = notificationChannel.SystemId,
            CallbackUrl = notificationChannel.CallbackUrl,
            Price = notificationChannel.Price,
            InfoUrl = notificationChannel.InfoUrl,
            Translations = notificationChannel.Translations
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.NotificationChannelsPending.Remove(notificationChannel);
                await _context.NotificationChannelsRejected.AddAsync(rejectedNotificationChannel);

                await _context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reject notification channel {Name}: Saving to DB failed.", notificationChannel.Name);
                transaction.Rollback();
                return new ServiceResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Error = "Something went wrong." };
            }
        }

        _logger.LogInformation("Notification channel rejected: {Name}", notificationChannel.Name);
        // Result
        return Ok(rejectedNotificationChannel.Id);
    }

    public async Task<ServiceResult<Guid>> ArchiveChannelAsync(ArchiveNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var notificationChannel = await _context.NotificationChannels.FindAsync(message.Id);
        if (notificationChannel is null)
        {
            _logger.LogInformation("NotificationChannel with this Id does not exist : {Id}.", message.Id);
            return NotFound<Guid>(nameof(message.Id), message.Id);
        }

        // SMTP deactivation is not allowed
        if (notificationChannel.Name == ConfigurationsConstants.SMTP)
        {
            _logger.LogInformation("Archiving NotificationChannel {Name} is not allowed.", notificationChannel.Name);
            return Conflict<Guid>(nameof(message.Id), message.Id, $"{notificationChannel.Name} archiving is not allowed");
        }

        var archiveNotificationChannel = new NotificationChannelArchive
        {
            Id = Guid.NewGuid(),
            ModifiedBy = message.ModifiedBy,
            ModifiedOn = DateTime.UtcNow,
            Name = notificationChannel.Name.Trim(),
            Description = notificationChannel.Description,
            SystemId = notificationChannel.SystemId,
            CallbackUrl = notificationChannel.CallbackUrl,
            Price = notificationChannel.Price,
            InfoUrl = notificationChannel.InfoUrl,
            Translations = notificationChannel.Translations,
            IsBuiltIn = notificationChannel.IsBuiltIn
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.NotificationChannels.Remove(notificationChannel);
                await _context.NotificationChannelsArchive.AddAsync(archiveNotificationChannel);

                await _context.SaveChangesAsync();

                transaction.Commit();
                await _publishEndpoint.Publish<NotificationChannelDeactivated>(new
                {
                    message.CorrelationId,
                    NotificationChannelId = notificationChannel.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Archive notification channel {Name}: Saving to DB failed.", notificationChannel.Name);
                transaction.Rollback();
                return new ServiceResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Error = "Something went wrong." };
            }
        }

        _logger.LogInformation("Notification channel archived: {Name}", notificationChannel.Name);
        // Result
        return Ok(archiveNotificationChannel.Id);
    }

    public async Task<ServiceResult<Guid>> RestoreChannelAsync(RestoreNotificationChannel message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var notificationChannel = await _context.NotificationChannelsArchive.FindAsync(message.Id);
        if (notificationChannel is null)
        {
            _logger.LogInformation("NotificationChannel with this Id does not exist : {Id}.", message.Id);
            return NotFound<Guid>(nameof(message.Id), message.Id);
        }

        var approvedChannelWithSameName = await _context.NotificationChannels
            .FirstOrDefaultAsync(nc => nc.Name.ToLower() == notificationChannel.Name.ToLower());

        if (approvedChannelWithSameName != null)
        {
            _logger.LogInformation("NotificationChannel with this name already exist : {Name}.", notificationChannel.Name);
            return Conflict<Guid>(nameof(notificationChannel.Name), notificationChannel.Name);
        }

        var approvedNotificationChannel = new NotificationChannelApproved
        {
            Id = Guid.NewGuid(),
            ModifiedBy = message.ModifiedBy,
            ModifiedOn = DateTime.UtcNow,
            Name = notificationChannel.Name.Trim(),
            Description = notificationChannel.Description,
            SystemId = notificationChannel.SystemId,
            CallbackUrl = notificationChannel.CallbackUrl,
            Price = notificationChannel.Price,
            InfoUrl = notificationChannel.InfoUrl,
            Translations = notificationChannel.Translations,
            IsBuiltIn = notificationChannel.IsBuiltIn
        };

        try
        {
            _context.NotificationChannelsArchive.Remove(notificationChannel);
            await _context.NotificationChannels.AddAsync(approvedNotificationChannel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification channel restored in approved state: {Name}", notificationChannel.Name);
            // Result
            return Ok(approvedNotificationChannel.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Restore notification channel {Name}: Saving to DB failed.", notificationChannel.Name);
            return new ServiceResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Error = "Something went wrong." };
        }
    }

    private static Entities.NotificationChannelTranslation CreateFromMessageNCTranslation(Contracts.Commands.NotificationChannelTranslation t)
    {
        var result = new Entities.NotificationChannelTranslation
        {
            Language = t.Language,
            Name = t.Name,
            Description = t.Description
        };

        return result;
    }
}
