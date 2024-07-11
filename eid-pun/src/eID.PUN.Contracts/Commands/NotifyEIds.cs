using MassTransit;

namespace eID.PUN.Contracts.Commands;

public interface NotifyEIds : CorrelatedBy<Guid>
{
    public IEnumerable<Guid> EIds { get; set; }
    public string EventCode { get; set; }
    public IEnumerable<Translation> Translations { get; set; }
}

public interface Translation
{
    public string Language { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
}
