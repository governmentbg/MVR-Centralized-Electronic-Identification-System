namespace eID.PAN.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
    {
        { (LogEventCode.SEND_EMAIL, LogEventLifecycle.REQUEST), "Заявка за изпращане на email" },
        { (LogEventCode.SEND_EMAIL, LogEventLifecycle.SUCCESS), "Успешно изпращане на email" },
        { (LogEventCode.SEND_EMAIL, LogEventLifecycle.FAIL), "Неуспешно изпращане на email" },
        { (LogEventCode.UPDATE_SMTP_CONFIGURATION, LogEventLifecycle.REQUEST), "Заявка за промяна на SMTP конфигурация" },
        { (LogEventCode.UPDATE_SMTP_CONFIGURATION, LogEventLifecycle.SUCCESS), "Успешна промяна на SMTP конфигурация" },
        { (LogEventCode.UPDATE_SMTP_CONFIGURATION, LogEventLifecycle.FAIL), "Неуспешна промяна на SMTP конфигурация" },
        { (LogEventCode.MODIFY_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за промяна на канал за нотификации" },
        { (LogEventCode.MODIFY_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешна промяна на канал за нотификации" },
        { (LogEventCode.MODIFY_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешна промяна на канал за нотификации" },
        { (LogEventCode.REGISTER_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за регистриране на канал за нотификации" },
        { (LogEventCode.REGISTER_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно регистриране на канал за нотификации" },
        { (LogEventCode.REGISTER_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно регистриране на канал за нотификации" },
        { (LogEventCode.APPROVE_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за одобрение на канал за нотификации" },
        { (LogEventCode.APPROVE_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно одобрение на канал за нотификации" },
        { (LogEventCode.APPROVE_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно одобрение на канал за нотификации" },
        { (LogEventCode.REJECT_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за отказване на канал за нотификации" },
        { (LogEventCode.REJECT_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно отказване на канал за нотификации" },
        { (LogEventCode.REJECT_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно отказване на канал за нотификации" },
        { (LogEventCode.ARCHIVE_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за архивиране на канал за нотификации" },
        { (LogEventCode.ARCHIVE_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно архивиране на канал за нотификации" },
        { (LogEventCode.ARCHIVE_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно архивиране на канал за нотификации" },
        { (LogEventCode.RESTORE_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за възстановяване на канал за нотификации" },
        { (LogEventCode.RESTORE_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно възстановяване на канал за нотификации" },
        { (LogEventCode.RESTORE_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно възстановяване на канал за нотификации" },
        { (LogEventCode.REGISTER_SYSTEM, LogEventLifecycle.REQUEST), "Заявка за регистриране на система" },
        { (LogEventCode.REGISTER_SYSTEM, LogEventLifecycle.SUCCESS), "Успешно регистриране на система" },
        { (LogEventCode.REGISTER_SYSTEM, LogEventLifecycle.FAIL), "Неуспешно регистриране на система" },
        { (LogEventCode.MODIFY_EVENT, LogEventLifecycle.REQUEST), "Заявка за промяна на събитие" },
        { (LogEventCode.MODIFY_EVENT, LogEventLifecycle.SUCCESS), "Успешна промяна на събитие" },
        { (LogEventCode.MODIFY_EVENT, LogEventLifecycle.FAIL), "Неуспешна промяна на събитие" },
        { (LogEventCode.REGISTER_USER_NOTIFICATION_CHANNELS, LogEventLifecycle.REQUEST), "Заявка за избор на канал за нотификации" },
        { (LogEventCode.REGISTER_USER_NOTIFICATION_CHANNELS, LogEventLifecycle.SUCCESS), "Успешен избор на канал за нотификации" },
        { (LogEventCode.REGISTER_USER_NOTIFICATION_CHANNELS, LogEventLifecycle.FAIL), "Неуспешен избор на канал за нотификации" },
        { (LogEventCode.GET_DEACTIVATED_USER_NOTIFICATIONS, LogEventLifecycle.REQUEST), "Заявка за извличане на деактивирани потребителски нотификации" },
        { (LogEventCode.GET_DEACTIVATED_USER_NOTIFICATIONS, LogEventLifecycle.SUCCESS), "Успешно извличане на деактивирани потребителски нотификации" },
        { (LogEventCode.GET_DEACTIVATED_USER_NOTIFICATIONS, LogEventLifecycle.FAIL), "Неуспешно извличане на деактивирани потребителски нотификации" },
        { (LogEventCode.REJECT_SYSTEM, LogEventLifecycle.REQUEST), "Заявка за отхвърляне на система" },
        { (LogEventCode.REJECT_SYSTEM, LogEventLifecycle.SUCCESS), "Успешно отхвърляне на система" },
        { (LogEventCode.REJECT_SYSTEM, LogEventLifecycle.FAIL), "Неуспешно отхвърляне на система" },
        { (LogEventCode.APPROVE_SYSTEM, LogEventLifecycle.REQUEST), "Заявка за одобрение на система" },
        { (LogEventCode.APPROVE_SYSTEM, LogEventLifecycle.SUCCESS), "Успешно одобрение на система" },
        { (LogEventCode.APPROVE_SYSTEM, LogEventLifecycle.FAIL), "Неуспешно одобрение на система" },
        { (LogEventCode.ARCHIVE_SYSTEM, LogEventLifecycle.REQUEST), "Заявка за архивиране на система" },
        { (LogEventCode.ARCHIVE_SYSTEM, LogEventLifecycle.SUCCESS), "Успешно архивиране на система" },
        { (LogEventCode.ARCHIVE_SYSTEM, LogEventLifecycle.FAIL), "Неуспешно архивиране на система" },
        { (LogEventCode.RESTORE_SYSTEM, LogEventLifecycle.REQUEST), "Заявка за възстановяване на система" },
        { (LogEventCode.RESTORE_SYSTEM, LogEventLifecycle.SUCCESS), "Успешно възстановяване на система" },
        { (LogEventCode.RESTORE_SYSTEM, LogEventLifecycle.FAIL), "Неуспешно възстановяване на система" },
        { (LogEventCode.SEND_SMS, LogEventLifecycle.REQUEST), "Заявка за изпращане на SMS" },
        { (LogEventCode.SEND_SMS, LogEventLifecycle.SUCCESS), "Успешно изпращане на SMS" },
        { (LogEventCode.SEND_SMS, LogEventLifecycle.FAIL), "Неуспешно изпращане на SMS" },
        { (LogEventCode.SEND_NOTIFICATION, LogEventLifecycle.REQUEST), "Заявка за изпращане на нотификация" },
        { (LogEventCode.SEND_NOTIFICATION, LogEventLifecycle.SUCCESS), "Успешно изпращане на нотификация" },
        { (LogEventCode.SEND_NOTIFICATION, LogEventLifecycle.FAIL), "Неуспешно изпращане на нотификация" },
        { (LogEventCode.CREATE_SMTP_CONFIGURATION, LogEventLifecycle.REQUEST), "Заявка за създаване на SMTP конфигурация" },
        { (LogEventCode.CREATE_SMTP_CONFIGURATION, LogEventLifecycle.SUCCESS), "Успешно създаване на SMTP конфигурация" },
        { (LogEventCode.CREATE_SMTP_CONFIGURATION, LogEventLifecycle.FAIL), "Неуспешно създаване на SMTP конфигурация" },
        { (LogEventCode.DELETE_SMTP_CONFIGURATION, LogEventLifecycle.REQUEST), "Заявка за изтриване на SMTP конфигурация" },
        { (LogEventCode.DELETE_SMTP_CONFIGURATION, LogEventLifecycle.SUCCESS), "Успешно изтриване на SMTP конфигурация" },
        { (LogEventCode.DELETE_SMTP_CONFIGURATION, LogEventLifecycle.FAIL), "Неуспешно изтриване на SMTP конфигурация" },
        { (LogEventCode.DEACTIVATE_USER_NOTIFICATIONS, LogEventLifecycle.REQUEST), "Заявка за деактивиране на потребителски нотификации" },
        { (LogEventCode.DEACTIVATE_USER_NOTIFICATIONS, LogEventLifecycle.SUCCESS), "Успешно деактивиране на потребителски нотификации" },
        { (LogEventCode.DEACTIVATE_USER_NOTIFICATIONS, LogEventLifecycle.FAIL), "Неуспешно деактивиране на потребителски нотификации" },
        { (LogEventCode.SEND_PUSH_NOTIFICATION, LogEventLifecycle.REQUEST), "Заявка за изпращане на нотификация към мобилно приложение" },
        { (LogEventCode.SEND_PUSH_NOTIFICATION, LogEventLifecycle.SUCCESS), "Успешно изпращане на нотификация към мобилно приложение" },
        { (LogEventCode.SEND_PUSH_NOTIFICATION, LogEventLifecycle.FAIL), "Неуспешно изпращане на нотификация към мобилно приложение" },
        { (LogEventCode.SEND_DIRECT_EMAIL, LogEventLifecycle.REQUEST), "Заявка за директно изпращане на email" },
        { (LogEventCode.SEND_DIRECT_EMAIL, LogEventLifecycle.SUCCESS), "Успешно директно изпращане на email" },
        { (LogEventCode.SEND_DIRECT_EMAIL, LogEventLifecycle.FAIL), "Неуспешно директно изпращане на email" },
        { (LogEventCode.TEST_NOTIFICATION_CHANNEL, LogEventLifecycle.REQUEST), "Заявка за тестване на канал за нотификации" },
        { (LogEventCode.TEST_NOTIFICATION_CHANNEL, LogEventLifecycle.SUCCESS), "Успешно тестване на канал за нотификации" },
        { (LogEventCode.TEST_NOTIFICATION_CHANNEL, LogEventLifecycle.FAIL), "Неуспешно тестване на канал за нотификации" }
    };

    public static string GetLogEventMessage(LogEventCode code, LogEventLifecycle state)
    {
        if (!LogEventMessages.Messages.ContainsKey((code, state)))
        {
            return string.Empty;
        }
        return Messages[(code, state)];
    }
}
