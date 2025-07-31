namespace eID.PUN.Service;

public static class Events
{
    private static readonly Event _carrierCreated = new() { Code = "PUN_C_Created", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Регистрирано Удостоверение за електронна идентичност и носител.", Description = "Регистрирано Удостоверение за електронна идентичност и носител." }, new EventTranslation { Language = "en", ShortDescription = "Registered certificate and carrier", Description = "Registered certificate and carrier" } } };
    private static readonly Event _carrierListAccessed = new() { Code = "PUN_C_List_Accessed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Направена е справка за Ваш носител на УЕИ.", Description = "Направена е справка за Ваш носител на Удостоверение за електронна идентичност." }, new EventTranslation { Language = "en", ShortDescription = "A check for your electronic identity carriers has been made.", Description = "A check for your electronic identity carriers has been made." } } };

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
