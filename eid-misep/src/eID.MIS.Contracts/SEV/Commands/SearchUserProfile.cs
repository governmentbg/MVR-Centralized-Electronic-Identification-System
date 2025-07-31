using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface SearchUserProfile : CorrelatedBy<Guid>
{
    public Guid EIdentityId { get; set; }
    public string Identifier { get; set; }
    public string TargetGroupId { get; set; }
}
