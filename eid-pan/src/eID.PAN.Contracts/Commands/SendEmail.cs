using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendEmail : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public IEnumerable<SendEmailTranslation> Translations { get; set; }
}

public interface SendEmailTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}
