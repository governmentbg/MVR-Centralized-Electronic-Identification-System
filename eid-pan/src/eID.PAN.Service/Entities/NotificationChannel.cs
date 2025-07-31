#nullable disable
using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public abstract class NotificationChannel : NotificationChannelResult, UserNotificationChannelResult
{
    public Guid Id { get; set; }
    public string SystemName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public string CallbackUrl { get; set; }
    public decimal Price { get; set; }
    public string Email { get; set; }
    public string InfoUrl { get; set; }
    public bool IsBuiltIn { get; set; } = false;

    public ICollection<NotificationChannelTranslation> Translations { get; set; } = new List<NotificationChannelTranslation>();

    public ICollection<UserNotificationChannel> UserNotificationChannels { get; set; } = new List<UserNotificationChannel>();

    IEnumerable<NotificationChannelTranslationResult> NotificationChannelResult.Translations { get => Translations; set => throw new NotImplementedException(); }
    IEnumerable<NotificationChannelTranslationResult> UserNotificationChannelResult.Translations { get => Translations; set => throw new NotImplementedException(); }
}
public class NotificationChannelApproved : NotificationChannel { }
public class NotificationChannelPending : NotificationChannel { }
public class NotificationChannelRejected : NotificationChannel, NotificationChannelRejectedResult
{
    public string Reason { get; set; }
}

public class NotificationChannelArchive : NotificationChannel { }
#nullable restore
