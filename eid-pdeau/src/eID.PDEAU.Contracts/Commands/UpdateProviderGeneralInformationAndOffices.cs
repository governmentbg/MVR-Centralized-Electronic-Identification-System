using eID.PDEAU.Contracts.Results;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface UpdateProviderGeneralInformationAndOffices : ProviderGeneralInformationAndOfficesResult, CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
