using eID.MIS.Contracts.SEV.External;
using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface SendMessageOnBehalf : CorrelatedBy<Guid>
{
    public Guid EIdentityId { get; set; }
    public SendMessageOnBehalfRequest Request { get; set; }
    public string SystemName { get; set; }
}
