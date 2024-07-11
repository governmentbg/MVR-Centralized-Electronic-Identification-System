using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendSms : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public IEnumerable<SendSmsTranslation> Translations { get; set; }
}

public interface SendSmsTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}

