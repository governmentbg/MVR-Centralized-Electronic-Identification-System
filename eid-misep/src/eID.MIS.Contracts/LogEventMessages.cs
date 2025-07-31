namespace eID.MIS.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
{
    { (LogEventCode.CREATE_PAYMENT_REQUEST, LogEventLifecycle.REQUEST), "Заявка за създаване на плащане" },
    { (LogEventCode.CREATE_PAYMENT_REQUEST, LogEventLifecycle.SUCCESS), "Успешно създаване на плащане" },
    { (LogEventCode.CREATE_PAYMENT_REQUEST, LogEventLifecycle.FAIL), "Неуспешно създаване на плащане" },

    { (LogEventCode.GET_PAYMENT_REQUEST_STATUS, LogEventLifecycle.REQUEST), "Заявка за извличане статуса на заявка за плащане" },
    { (LogEventCode.GET_PAYMENT_REQUEST_STATUS, LogEventLifecycle.SUCCESS), "Успешно извличане статуса на заявка за плащане" },
    { (LogEventCode.GET_PAYMENT_REQUEST_STATUS, LogEventLifecycle.FAIL), "Неуспешно извличане статуса на заявка за плащане" },

    { (LogEventCode.GET_PAYMENT_REQUESTS, LogEventLifecycle.REQUEST), "Заявка за извличане на плащане" },
    { (LogEventCode.GET_PAYMENT_REQUESTS, LogEventLifecycle.SUCCESS), "Успешно извличане на плащане" },
    { (LogEventCode.GET_PAYMENT_REQUESTS, LogEventLifecycle.FAIL), "Неуспешно извличане на плащане" },

    { (LogEventCode.GET_CLIENTS_BY_EIK, LogEventLifecycle.REQUEST), "Заявка за изличане на клиенти по ЕИК от системата за електронни плащания" },
    { (LogEventCode.GET_CLIENTS_BY_EIK, LogEventLifecycle.SUCCESS), "Успешно изличане на клиенти по ЕИК от системата за електронни плащания" },
    { (LogEventCode.GET_CLIENTS_BY_EIK, LogEventLifecycle.FAIL), "Неуспешно изличане на клиенти по ЕИК от системата за електронни плащания" },

    { (LogEventCode.CREATE_PASSIVE_INDIVIDUAL_PROFILE, LogEventLifecycle.REQUEST), "Заявка за създаване на пасивен профил в системата за сигурно електронно връчване" },
    { (LogEventCode.CREATE_PASSIVE_INDIVIDUAL_PROFILE, LogEventLifecycle.SUCCESS), "Успешно създаване на пасивен профил в системата за сигурно електронно връчване" },
    { (LogEventCode.CREATE_PASSIVE_INDIVIDUAL_PROFILE, LogEventLifecycle.FAIL), "Неуспешно създаване на пасивен профил в системата за сигурно електронно връчване" },

    { (LogEventCode.SEARCH_PROFILE, LogEventLifecycle.REQUEST), "Заявка за търсене на профил в системата за сигурно електронно връчване" },
    { (LogEventCode.SEARCH_PROFILE, LogEventLifecycle.SUCCESS), "Успешно търсене на профил в системата за сигурно електронно връчване" },
    { (LogEventCode.SEARCH_PROFILE, LogEventLifecycle.FAIL), "Неуспешно търсене на профил в системата за сигурно електронно връчване" },

    { (LogEventCode.GET_PROFILE, LogEventLifecycle.REQUEST), "Заявка за извличане на профил от системата за сигурно електронно връчване" },
    { (LogEventCode.GET_PROFILE, LogEventLifecycle.SUCCESS), "Успешно извличане на профил от системата за сигурно електронно връчване" },
    { (LogEventCode.GET_PROFILE, LogEventLifecycle.FAIL), "Неуспешно извличане на профил от системата за сигурно електронно връчване" },

    { (LogEventCode.SEND_MESSAGE_ON_BEHALF, LogEventLifecycle.REQUEST), "Заявка за изпращане на съобщение чрез системата за сигурно електронно връчване от друго име" },
    { (LogEventCode.SEND_MESSAGE_ON_BEHALF, LogEventLifecycle.SUCCESS), "Успешно изпращане на съобщение чрез системата за сигурно електронно връчване от друго име" },
    { (LogEventCode.SEND_MESSAGE_ON_BEHALF, LogEventLifecycle.FAIL), "Неуспешно изпращане на съобщение чрез системата за сигурно електронно връчване от друго име" },

    { (LogEventCode.UPLOAD_BLOB, LogEventLifecycle.REQUEST), "Заявка за качване насъдържание в системата за сигурно електронно връчване" },
    { (LogEventCode.UPLOAD_BLOB, LogEventLifecycle.SUCCESS), "Успешно качване насъдържание в системата за сигурно електронно връчване" },
    { (LogEventCode.UPLOAD_BLOB, LogEventLifecycle.FAIL), "Неуспешно качване насъдържание в системата за сигурно електронно връчване" },

    { (LogEventCode.GET_DELIVERIES, LogEventLifecycle.REQUEST), "Заявка за извличане на връчени съобщения чрез системата за сигурно електронно връчване" },
    { (LogEventCode.GET_DELIVERIES, LogEventLifecycle.SUCCESS), "Успешно извличане на връчени съобщения чрез системата за сигурно електронно връчване" },
    { (LogEventCode.GET_DELIVERIES, LogEventLifecycle.FAIL), "Неуспешно извличане на връчени съобщения чрез системата за сигурно електронно връчване" },

    { (LogEventCode.SEND_MESSAGE, LogEventLifecycle.REQUEST), "Заявка за изпращане на съобщение чрез системата за сигурно електронно връчване" },
    { (LogEventCode.SEND_MESSAGE, LogEventLifecycle.SUCCESS), "Успешно изпращане на съобщение чрез системата за сигурно електронно връчване" },
    { (LogEventCode.SEND_MESSAGE, LogEventLifecycle.FAIL), "Неуспешно изпращане на съобщение чрез системата за сигурно електронно връчване" },

    { (LogEventCode.UPLOAD_BLOB_ON_BEHALF_OF, LogEventLifecycle.REQUEST), "Заявка за качване насъдържание в системата за сигурно електронно връчване от друго име" },
    { (LogEventCode.UPLOAD_BLOB_ON_BEHALF_OF, LogEventLifecycle.SUCCESS), "Успешно качване насъдържание в системата за сигурно електронно връчване от друго име" },
    { (LogEventCode.UPLOAD_BLOB_ON_BEHALF_OF, LogEventLifecycle.FAIL), "Неуспешно качване насъдържание в системата за сигурно електронно връчване от друго име" }
};


    public static string GetLogEventMessage(LogEventCode code, LogEventLifecycle state)
    {
        if (Messages.TryGetValue((code, state), out var result))
        {
            return result;
        }

        return string.Empty;
    }
}
