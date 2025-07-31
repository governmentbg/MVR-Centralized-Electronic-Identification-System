using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface GetUserProfile : CorrelatedBy<Guid>
{
    public Guid EIdentityId { get; set; }
    public string ProfileId { get; set; }
}
