using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface ModifyEvent : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
}
