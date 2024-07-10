using MassTransit;

namespace eID.POD.Contracts.Commands;

public interface DeleteDataset : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string LastModifiedBy { get; set; }
}
