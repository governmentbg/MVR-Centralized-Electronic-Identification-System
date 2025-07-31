using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface DenyService : CorrelatedBy<Guid>
{
    public Guid ServiceId { get; set; }
    public string DenialReason { get; set; }
    public string ReviewerFullName { get; set; }
    public bool IsPLSRole { get; set; }
}
