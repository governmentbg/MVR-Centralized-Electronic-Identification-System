namespace eID.PUN.Service;

public static class Events
{
    private static readonly Event _carrierCreated = new() { Code = "PUN_C_Created", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Регистриранo удостоверение и носител", Description = "Регистриранo удостоверение и носител" }, new EventTranslation { Language = "en", ShortDescription = "Registered certificate and carrier", Description = "Registered certificate and carrier" } } };
    private static readonly Event _carrierListAccessed = new() { Code = "PUN_C_List_Accessed", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Осъществен достъп до списък с носители", Description = "Осъществен достъп до списък с носители" }, new EventTranslation { Language = "en", ShortDescription = "Carriers list accessed", Description = "Carriers list accessed" } } };

    public static Event CarrierCreated { get { return _carrierCreated; } }
    public static Event CarrierListAccessed { get { return _carrierListAccessed; } }

    public static List<Event> GetAllEvents()
    {
        var allEvents = new List<Event> {
            _carrierCreated,
            _carrierListAccessed,
        };

        var allEventsGotUniqueName = allEvents.DistinctBy(x => x.Code).Count() == allEvents.Count;
        if (!allEventsGotUniqueName)
        {
            throw new ArgumentException("PUN events have duplicated codes");
        }

        return allEvents;
    }
}

public class Event
{
    public string Code { get; init; }
    public bool IsMandatory { get; init; }
    public List<EventTranslation> Translations { get; init; }
}

public class EventTranslation
{
    public string Language { get; init; }
    public string Description { get; init; }
    public string ShortDescription { get; init; }
}
