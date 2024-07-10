namespace eID.RO.Service.EventsRegistration;

public static class Events
{
    private static readonly Event _empowermentCreated = new() { Code = "RO_E_Created", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е регистрирано", Description = "Вашето овластяване е регистрирано" }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment was registered", Description = "Your empowerment was registered" } } };

    private static readonly Event _empowermentCompleted = new() { Code = "RO_E_Completed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е вписано", Description = "Вашето овластяване е вписано" }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been recorded", Description = "Your empowerment has been recorded" } } };
    private static readonly Event _empowermentToMeCompleted = new() { Code = "RO_E_TO_ME_Completed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Имате ново овластяване към Вас", Description = "Имате ново овластяване към Вас" }, new EventTranslation { Language = "en", ShortDescription = "You have a new empowerment", Description = "You have a new empowerment" } } };

    private static readonly Event _empowermentWasDisagreed = new() { Code = "RO_E_Was_Disagreed", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Декларирано е несъгласие с Ваше овластяване", Description = "Декларирано е несъгласие с Ваше овластяване" }, new EventTranslation { Language = "en", ShortDescription = "A disagreement with your еmpowerment has been declared", Description = "A disagreement with your еmpowerment has been declared" } } };
    private static readonly Event _empowermentToMeWasDisagreed = new() { Code = "RO_E_TO_ME_Was_Disagreed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Декларирано е несъгласие с овластяване към Вас", Description = "Декларирано е несъгласие с овластяване към Вас" }, new EventTranslation { Language = "en", ShortDescription = "A disagreement with the еmpowerment for you has been declared", Description = "A disagreement with the еmpowerment for you has been declared" } } };

    private static readonly Event _withdrawalTimeout = new() { Code = "RO_E_Withdrawal_Timeout", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Оттеглянето е прекратено, поради неуспешно събиране на съгласия", Description = "Оттеглянето на Вашето овластяване е прекратено, поради неуспешно събиране на съгласия от всички овластители" }, new EventTranslation { Language = "en", ShortDescription = "The withdrawal has been terminated due to lack of consent", Description = "The withdrawal of your еmpowerment has been terminated due to unsuccessful gathering of consents from all authorizers" } } };
    private static readonly Event _withdrawalCollectConfirmations = new() { Code = "RO_E_Withdrawal_Collect_Confirmation", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Оттеглянето очаква потвърждение от всички овластители", Description = "Оттеглянето на Вашето овластяване очаква потвърждение от всички овластители по него" }, new EventTranslation { Language = "en", ShortDescription = "The withdrawal requires confirmation from all authorizers", Description = "The withdrawal of your еmpowerment requires confirmation from all authorizers" } } };
    private static readonly Event _withdrawalToMeCollectConfirmations = new() { Code = "RO_E_Withdrawal_TO_ME_Collect_Confirmation", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Ваше овластяване очаква потвърждение за оттегляне", Description = "Ваше овластяване очаква потвърждение за оттегляне" }, new EventTranslation { Language = "en", ShortDescription = "Your еmpowerment requires confirmation for withdrawal", Description = "Your еmpowerment requires confirmation for withdrawal" } } };
    private static readonly Event _withdrawalDeclined = new() { Code = "RO_E_Withdrawal_Declined", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Оттеглянето е отказано поради некоректни данни", Description = "Оттеглянето на Вашето овластяване е отказано, поради некоректни данни" }, new EventTranslation { Language = "en", ShortDescription = "The withdrawal has been denied due to incorrect information", Description = "The withdrawal of your еmpowerment has been denied due to incorrect information" } } };

    private static readonly Event _empowermentWasWithdrawn = new() { Code = "RO_E_Was_Withdrawn", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване беше оттеглено успешно", Description = "Вашето овластяване беше оттеглено успешно" }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been successfully revoked", Description = "Your empowerment has been successfully revoked" } } };
    private static readonly Event _empowermentToMeWasWithdrawn = new() { Code = "RO_E_TO_ME_Was_Withdrawn", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяване към Вас е оттеглено", Description = "Овластяване към Вас е оттеглено" }, new EventTranslation { Language = "en", ShortDescription = "Еmpowerment for yourself has been revoked", Description = "Еmpowerment for yourself has been revoked" } } };
    private static readonly Event _empowermentDeclined = new() { Code = "RO_E_Declined", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяването е отказано поради некоректни данни в изявлението", Description = "Вашето овластяване е отказано, поради некоректни данни в изявлението" }, new EventTranslation { Language = "en", ShortDescription = "The empowerment has been denied due to incorrect information", Description = "Your empowerment has been denied due to incorrect information in the statement" } } };

    private static readonly Event _empowermentNeedsSignature = new() { Code = "RO_E_Needs_Signature", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Имате ново овластяване за подписване", Description = "Имате ново овластяване за подписване" }, new EventTranslation { Language = "en", ShortDescription = "You have a new empowerment for signing", Description = "You have a new empowerment for signing" } } };
    private static readonly Event _empowermentSigned = new() { Code = "RO_E_Signed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е подписано", Description = "Вашето овластяване е подписано" }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been signed", Description = "Your empowerment has been signed" } } };
    private static readonly Event _empowermentTimeout = new() { Code = "RO_E_Timeout", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяването не е вписано поради неуспешно събиране на подписи", Description = "Вашето овластяване не е вписано, поради неуспешно събиране на подписи от всички овластители." }, new EventTranslation { Language = "en", ShortDescription = "The еmpowerment has not been recorded because it was not signed", Description = "Your еmpowerment has not been recorded due to unsuccessful collection of signatures from all authorizers" } } };
    
    private static readonly Event _empowermentExpiringSoon = new() { Code = "RO_E_ExpiringSoon", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Имате скоро изтичащо овластяване", Description = "Има скоро изтичащо овластяване" }, new EventTranslation { Language = "en", ShortDescription = "You have an Empowerment statement that is about to expire", Description = "You have an Empowerment statement that is about to expire" } } };

    public static Event EmpowermentCreated { get { return _empowermentCreated; } }
    public static Event EmpowermentCompleted { get { return _empowermentCompleted; } }
    public static Event EmpowermentToMeCompleted { get { return _empowermentToMeCompleted; } }
    public static Event EmpowermentWasDisagreed { get { return _empowermentWasDisagreed; } }
    public static Event EmpowermentToMeWasDisagreed { get { return _empowermentToMeWasDisagreed; } }
    public static Event EmpowermentDeclined { get { return _empowermentDeclined; } }
    public static Event EmpowermentToMeWasWithdrawn { get { return _empowermentToMeWasWithdrawn; } }
    public static Event EmpowermentWasWithdrawn { get { return _empowermentWasWithdrawn; } }
    public static Event WithdrawalTimeout { get { return _withdrawalTimeout; } }
    public static Event WithdrawalDeclined { get { return _withdrawalDeclined; } }
    public static Event WithdrawalCollectConfirmations { get { return _withdrawalCollectConfirmations; } }
    public static Event WithdrawalToMeCollectConfirmations { get { return _withdrawalToMeCollectConfirmations; } }
    public static Event EmpowermentNeedsSignature { get { return _empowermentNeedsSignature; } }
    public static Event EmpowermentSigned { get { return _empowermentSigned; } }
    public static Event EmpowermentTimeout { get { return _empowermentTimeout; } }
    public static Event EmpowermentExpiringSoon { get { return _empowermentExpiringSoon; } }

    public static List<Event> GetAllEvents()
    {
        var allEvents = new List<Event> {
            _empowermentCreated,
            _empowermentCompleted, _empowermentToMeCompleted,
            _empowermentWasDisagreed, _empowermentToMeWasDisagreed,
            _withdrawalCollectConfirmations, _withdrawalToMeCollectConfirmations,
            _withdrawalTimeout, _withdrawalDeclined,
            _empowermentToMeWasWithdrawn, _empowermentWasWithdrawn, _empowermentDeclined,
            _empowermentNeedsSignature, _empowermentSigned, _empowermentTimeout, _empowermentExpiringSoon
        };

        var allEventsGotUniqueName = allEvents.DistinctBy(x => x.Code).Count() == allEvents.Count;
        if (!allEventsGotUniqueName)
        {
            throw new ArgumentException("RO events have duplicated codes");
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
