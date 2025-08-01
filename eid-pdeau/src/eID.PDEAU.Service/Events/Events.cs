namespace eID.PDEAU.Service.Events;

public static class Events
{
    #region Events description
    private static readonly Event _deau_AdminConfirmationEmail = new()
    {
        Code = "DEAU_Admin_Confirmation_Email",
        IsMandatory = true,
        Translations = new List<EventTranslation>
        {
            new()
            {
                Language = "bg",
                ShortDescription = "Потвърждение в повишаване в Администратор.",
                Description = "Потвърждение в повишаване в Администратор. Моля потвърдете като натиснете следния линк: {{ ConfirmationLink }}"
            },
            new()
            {
                Language = "en",
                ShortDescription = "Confirmation in promotion to Administrator.",
                Description = "Confirmation in promotion to Administrator. Please confirm by clicking: {{ ConfirmationLink }}"
            }
        }
    };
    #endregion

    public static Event AdminPromotionConfirmationEmail { get { return _deau_AdminConfirmationEmail; } }

    public static List<Event> GetAllEvents()
    {
        var allEvents = new List<Event> {
            _deau_AdminConfirmationEmail
        };

        var allEventsGotUniqueName = allEvents.DistinctBy(x => x.Code).Count() == allEvents.Count;
        if (!allEventsGotUniqueName)
        {
            throw new ArgumentException("PDEAU events have duplicated codes");
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
    /// <summary>
    /// "bg" and "en"
    /// </summary>
    public string Language { get; init; }

    /// <summary>
    /// Max length 128
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Max length 64
    /// </summary>
    public string ShortDescription { get; init; }
}
