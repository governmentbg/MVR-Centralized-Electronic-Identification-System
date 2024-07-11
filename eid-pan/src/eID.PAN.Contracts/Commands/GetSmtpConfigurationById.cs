using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetSmtpConfigurationById : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
