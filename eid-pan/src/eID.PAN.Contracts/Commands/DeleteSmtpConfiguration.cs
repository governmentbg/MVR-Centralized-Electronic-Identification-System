using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface DeleteSmtpConfiguration : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
