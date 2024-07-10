using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendHttpCallbackAsync : CorrelatedBy<Guid>
{
    public string UserId { get; set; }
    public string CallbackUrl { get; set; }
    public object Body { get; set; }

    public IEnumerable<SendHttpCallbackTranslation> Translations { get; set; }
}

public interface SendHttpCallbackTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}
