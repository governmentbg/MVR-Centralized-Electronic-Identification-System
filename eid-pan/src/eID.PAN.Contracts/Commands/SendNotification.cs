using eID.PAN.Contracts.Enums;
using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendNotification : CorrelatedBy<Guid>
{
    string SystemName { get; }
    string EventCode { get; }
    Guid? UserId { get; }
    Guid? EId { get; }
    string Uid { get; }
    IdentifierType UidType { get; }
    IEnumerable<SendNotificationTranslation> Translations { get; }
}

public interface SendNotificationTranslation
{
    string Language { get; }
    string Message { get; }
}
