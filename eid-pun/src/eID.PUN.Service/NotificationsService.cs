using eID.PUN.Contracts.Commands;
using eID.PUN.Contracts.Results;
using eID.PUN.Service.Validators;
using Microsoft.Extensions.Logging;

namespace eID.PUN.Service;

public class NotificationsService : BaseService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly INotificationsSender _notificationSender;

    public NotificationsService(
        ILogger<NotificationsService> logger,
        INotificationsSender notificationSender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
    }

    public async Task<ServiceResult<bool>> SendAsync(NotifyEIds message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new NotifyEIdsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Notification Sender service validation failed. {Errors}", validationResult.Errors);
            return BadRequest<bool>(validationResult.Errors);
        }

        var isSuccessfullySend = await _notificationSender.SendAsync(message);
        if (!isSuccessfullySend)
        {
            _logger.LogWarning("Sending notifications completed with errors");
        } else
        {
            _logger.LogInformation("Sending notifications completed successfully");
        }

        return Ok(isSuccessfullySend);
    }
}
