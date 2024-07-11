using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RegisterSystem : CorrelatedBy<Guid>
{
    public string SystemName { get; set; }
    public string ModifiedBy { get; set; }
    public IEnumerable<RegisteredSystemTranslation> Translations { get; set; }

    public IEnumerable<SystemEvent> Events { get; set; }
}

public interface SystemEvent
{
    public string Code { get; set; }
    public bool IsMandatory { get; set; }

    public IEnumerable<Translation> Translations { get; set; }
}

public interface Translation
{
    public string Language { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
}

public interface RegisteredSystemTranslation
{
    public string Language { get; set; }
    public string Name { get; set; }
}
