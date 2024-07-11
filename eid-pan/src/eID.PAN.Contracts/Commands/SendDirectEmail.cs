using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendDirectEmail : CorrelatedBy<Guid>
{
    public string Language { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string EmailAddress { get; set; }
}
