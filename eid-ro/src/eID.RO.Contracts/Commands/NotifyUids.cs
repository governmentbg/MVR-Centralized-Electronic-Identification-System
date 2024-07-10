using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface NotifyUids : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    public IEnumerable<UserIdentifier> Uids { get; set; }
    public string EventCode { get; set; }
    public IEnumerable<Translation> Translations { get; set; }
}

public interface Translation
{
    public string Language { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
}
