using eID.MIS.Contracts.SEV.External;
using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface CreatePassiveIndividualProfile : CorrelatedBy<Guid>
{
    public Guid EIdentityId { get; set; }
    public CreatePassiveIndividualProfileRequest Request { get; set; }
    public string SystemName { get; set; }
}
