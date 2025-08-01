using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderGeneralInformationAndOfficesById : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
