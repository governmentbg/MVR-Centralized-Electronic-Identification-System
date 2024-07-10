using MassTransit;

namespace eID.POD.Contracts.Commands;

public interface ManualUploadDataset : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
