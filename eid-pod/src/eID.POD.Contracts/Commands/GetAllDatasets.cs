using MassTransit;

namespace eID.POD.Contracts.Commands;

public interface GetAllDatasets : CorrelatedBy<Guid>
{
}
