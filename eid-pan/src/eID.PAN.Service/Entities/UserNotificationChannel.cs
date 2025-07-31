#nullable disable
namespace eID.PAN.Service.Entities;

public class UserNotificationChannel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid NotificationChannelId { get; set; }
    public NotificationChannel Channel { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
}
#nullable restore
