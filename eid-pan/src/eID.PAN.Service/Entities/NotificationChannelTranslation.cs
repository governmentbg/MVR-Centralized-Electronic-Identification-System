using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public class NotificationChannelTranslation : NotificationChannelTranslationResult
{ 
    public string Language { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
