using eID.RO.Contracts.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace eID.RO.Service.EventsRegistration;

public static class Events
{
    private static readonly Event _empowermentCreated = new() { Code = "RO_E_Created", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е подадено.", Description = "Вашето овластяване е подадено." }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been submitted.", Description = "Your empowerment has been submitted." } } };

    private static readonly Event _empowermentCompleted = new() { Code = "RO_E_Completed", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е вписано в Регистъра на овластяванията.", Description = "Вашето овластяване е вписано в Регистъра на овластяванията." }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been written in the Authorization register.", Description = "Your empowerment has been recorded in the Authorization register." } } };
    private static readonly Event _empowermentToMeCompleted = new() { Code = "RO_E_TO_ME_Completed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Имате ново овластяване към Вас.", Description = "Имате ново овластяване към Вас. Можете да прегледате овластяването в Портала на гражданите." }, new EventTranslation { Language = "en", ShortDescription = "You have a new empowerment.", Description = "You have a new empowerment. You can review the empowerment in the Citizen Portal." } } };

    private static readonly Event _empowermentWasDisagreed = new() { Code = "RO_E_Was_Disagreed", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Декларирано е несъгласие с Ваше овластяване.", Description = "Декларирано е несъгласие с Ваше овластяване." }, new EventTranslation { Language = "en", ShortDescription = "A disagreement with your empowerment has been declared.", Description = "A disagreement with your empowerment has been declared." } } };
    private static readonly Event _empowermentToMeWasDisagreed = new() { Code = "RO_E_TO_ME_Was_Disagreed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Декларирано е несъгласие с овластяване към Вас.", Description = "Декларирано е несъгласие с овластяване към Вас." }, new EventTranslation { Language = "en", ShortDescription = "A disagreement with the empowerment for you has been declared.", Description = "A disagreement with the empowerment for you has been declared." } } };

    private static readonly Event _withdrawalTimeout = new() { Code = "RO_E_Withdrawal_Timeout", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Оттеглянето е прекратено, поради изтичане на срока за оттегляне.", Description = "Оттеглянето е прекратено, поради изтичане на срока за оттегляне." }, new EventTranslation { Language = "en", ShortDescription = "The withdrawal was cancelled due to the expiration.", Description = "The withdrawal has been cancelled due to the expiration of the withdrawal period." } } };
    private static readonly Event _withdrawalDeclined = new() { Code = "RO_E_Withdrawal_Declined", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Оттеглянето на Вашето овластяване е отказано.", Description = "Оттеглянето на Вашето овластяване е отказано." }, new EventTranslation { Language = "en", ShortDescription = "The withdrawal of your empowerment has been denied.", Description = "The withdrawal of your empowerment has been denied." } } };

    private static readonly Event _empowermentWasWithdrawn = new() { Code = "RO_E_Was_Withdrawn", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване беше оттеглено успешно.", Description = "Вашето овластяване беше оттеглено успешно." }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been successfully revoked.", Description = "Your empowerment has been successfully revoked." } } };
    private static readonly Event _empowermentToMeWasWithdrawn = new() { Code = "RO_E_TO_ME_Was_Withdrawn", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяване към Вас е оттеглено.", Description = "Овластяване към Вас е оттеглено." }, new EventTranslation { Language = "en", ShortDescription = "Еmpowerment for yourself has been revoked.", Description = "Empowerment for yourself has been revoked." } } };
    private static readonly Event _empowermentDeclined = new() { Code = "RO_E_Declined", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е отказано, поради некоректни данни.", Description = "Вашето овластяване е отказано, поради некоректни данни в изявлението." }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been denied due to incorrect information.", Description = "Your empowerment has been denied due to incorrect information in the statement." } } };

    private static readonly Event _empowermentNeedsSignature = new() { Code = "RO_E_Needs_Signature", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Имате ново овластяване за подписване като овластител.", Description = "Имате ново овластяване за подписване в качеството Ви на овластител." }, new EventTranslation { Language = "en", ShortDescription = "You have a new empowerment to sign as the authorizing party.", Description = "You have a new empowerment to sign in your capacity as the authorizing party." } } };
    private static readonly Event _empowermentSigned = new() { Code = "RO_E_Signed", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Вашето овластяване е подписано.", Description = "Вашето овластяване е подписано." }, new EventTranslation { Language = "en", ShortDescription = "Your empowerment has been signed.", Description = "Your empowerment has been signed." } } };
    private static readonly Event _empowermentTimeout = new() { Code = "RO_E_Timeout", IsMandatory = true, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяването не е активно поради неуспешно събиране на подписи.", Description = "Вашето овластяване не е активно, поради неуспешно събиране на подписи от всички овластители." }, new EventTranslation { Language = "en", ShortDescription = "The empowerment has not been activated because it wasn't signed.", Description = "Your empowerment has not been activated due to unsuccessful collection of signatures from all authorizers." } } };

    private static readonly Event _empowermentExpiringSoon = new() { Code = "RO_E_ExpiringSoon", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Овластяване от/към Вас изтича скоро.", Description = "Овластяване от/към Вас изтича скоро." }, new EventTranslation { Language = "en", ShortDescription = "An empowerment from/to you is about to expire.", Description = "An empowerment from/to you is about to expire." } } };

    private static readonly Event _empowermentIsBeingCheckedByDEAU = new() { Code = "RO_E_IsBeingCheckedByDEAU", IsMandatory = false, Translations = new List<EventTranslation> { new EventTranslation { Language = "bg", ShortDescription = "Ваше овластяване е проверявано от ДЕАУ.", Description = "Ваше овластяване е проверявано от Доставчик на електронни административни услуги." }, new EventTranslation { Language = "en", ShortDescription = "You Empowerment is being checked by SEAS.", Description = "You empowerment is being checked by supplier of electronic administrative service." } } };

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
    public static Event EmpowermentNeedsSignature { get { return _empowermentNeedsSignature; } }
    public static Event EmpowermentSigned { get { return _empowermentSigned; } }
    public static Event EmpowermentTimeout { get { return _empowermentTimeout; } }
    public static Event EmpowermentExpiringSoon { get { return _empowermentExpiringSoon; } }
    public static Event EmpowermentIsBeingCheckedByDEAU { get { return _empowermentIsBeingCheckedByDEAU; } }

    public static List<Event> GetAllEvents(ILogger logger)
    {
        var allEvents = new List<Event> {
            _empowermentCreated,
            _empowermentCompleted, _empowermentToMeCompleted,
            _empowermentWasDisagreed, _empowermentToMeWasDisagreed,
            _withdrawalTimeout, _withdrawalDeclined,
            _empowermentToMeWasWithdrawn, _empowermentWasWithdrawn, _empowermentDeclined,
            _empowermentNeedsSignature, _empowermentSigned, _empowermentTimeout, _empowermentExpiringSoon,
            _empowermentIsBeingCheckedByDEAU
        };

        var allEventsGotUniqueName = allEvents.DistinctBy(x => x.Code).Count() == allEvents.Count;
        if (!allEventsGotUniqueName)
        {
            throw new ArgumentException("RO events have duplicated codes");
        }
        
        Validate(logger, allEvents);

        return allEvents;
    }

    private static void Validate(ILogger logger, List<Event> allEvents)
    {
        // Validate
        var errors = string.Empty;
        var eventValidator = new EventValidator();
        foreach (var @event in allEvents)
        {
            var result = eventValidator.Validate(@event);
            if (!result.IsValid)
            {
                logger.LogError("Event code: {event} {errors}", @event.Code, result.ToString(";"));
            }
        }
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

internal class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(r => r.Code).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Translations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .Must(t => t.Count() >= 2)
            .Must(t => t.Any(c => c.Language == "bg") && t.Any(c => c.Language == "en"))
                .WithMessage($"{nameof(Translation)} languages must contain with 'bg' and 'en'")
            .ForEach(r => r.SetValidator(new EventTranslationValidator()));
    }
}

internal class EventTranslationValidator : AbstractValidator<EventTranslation>
{
    public EventTranslationValidator()
    {
        RuleFor(r => r.Language).NotEmpty().MinimumLength(2).MaximumLength(2);
        RuleFor(r => r.ShortDescription).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(2).MaximumLength(128);
    }
}
