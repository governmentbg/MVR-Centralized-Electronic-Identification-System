using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using eID.RO.Service.Validators;
using Microsoft.Extensions.Logging;

namespace eID.RO.Service;

public class NotificationSenderService : BaseService
{
    private readonly ILogger<NotificationSenderService> _logger;
    private readonly INotificationSender _notificationSender;

    public NotificationSenderService(
        ILogger<NotificationSenderService> logger,
        INotificationSender notificationSender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
    }

    public async Task<ServiceResult<bool>> SendAsync(NotifyUids notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        // Validation
        var validator = new NotifyUidsValidator();
        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Notification Sender service validation failed. {Errors}", validationResult.Errors);
            return BadRequest<bool>(validationResult.Errors);
        }

        var isSuccessfullySend = await _notificationSender.SendAsync(notification);
        _logger.LogInformation("Sending notifications completed successfully");

        return Ok(isSuccessfullySend);
    }
}
