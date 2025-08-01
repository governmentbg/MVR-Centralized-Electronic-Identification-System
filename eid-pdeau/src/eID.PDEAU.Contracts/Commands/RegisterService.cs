using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface RegisterService : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid ProviderDetailsId { get; set; }
    public Guid UserId { get; set; }
    public bool IsEmpowerment { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
    decimal? PaymentInfoNormalCost { get; set; }
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
    public IEnumerable<string> ServiceScopeNames { get; set; }
}
