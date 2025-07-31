namespace eID.Signing.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
{
    { (LogEventCode.EVROTRUST_SIGN_DOCUMENT, LogEventLifecycle.REQUEST), "Заявка за подписване на документ чрез Евротръст" },
    { (LogEventCode.EVROTRUST_SIGN_DOCUMENT, LogEventLifecycle.SUCCESS), "Успешно подписване на документ чрез Евротръст" },
    { (LogEventCode.EVROTRUST_SIGN_DOCUMENT, LogEventLifecycle.FAIL), "Неуспешно подписване на документ чрез Евротръст" },

    { (LogEventCode.EVROTRUST_GET_FILE_STATUS, LogEventLifecycle.REQUEST), "Заявка за извличане на статус по документ изпратен за подписване чрез Евротръст" },
    { (LogEventCode.EVROTRUST_GET_FILE_STATUS, LogEventLifecycle.SUCCESS), "Успешно извличане на статус по документ изпратен за подписване чрез Евротръст" },
    { (LogEventCode.EVROTRUST_GET_FILE_STATUS, LogEventLifecycle.FAIL), "Неуспешно извличане на статус по документ изпратен за подписване чрез Евротръст" },

    { (LogEventCode.EVROTRUST_DOWNLOAD_FILE, LogEventLifecycle.REQUEST), "Заявка за сваляне на подписан документ чрез Евротръст" },
    { (LogEventCode.EVROTRUST_DOWNLOAD_FILE, LogEventLifecycle.SUCCESS), "Успешно сваляне на подписан документ чрез Евротръст" },
    { (LogEventCode.EVROTRUST_DOWNLOAD_FILE, LogEventLifecycle.FAIL), "Неуспешно сваляне на подписан документ чрез Евротръст" },

    { (LogEventCode.EVROTRUST_CHECK_USER, LogEventLifecycle.REQUEST), "Заявка за проверка на гражданин в Евротръст" },
    { (LogEventCode.EVROTRUST_CHECK_USER, LogEventLifecycle.SUCCESS), "Успешна проверка на гражданин в Евротръст" },
    { (LogEventCode.EVROTRUST_CHECK_USER, LogEventLifecycle.FAIL), "Неуспешна проверка на гражданин в Евротръст" },

    { (LogEventCode.BORICA_SIGN_DOCUMENT, LogEventLifecycle.REQUEST), "Заявка за подписване на документ чрез Борика" },
    { (LogEventCode.BORICA_SIGN_DOCUMENT, LogEventLifecycle.SUCCESS), "Успешно подписване на документ чрез Борика" },
    { (LogEventCode.BORICA_SIGN_DOCUMENT, LogEventLifecycle.FAIL), "Неуспешно подписване на документ чрез Борика" },

    { (LogEventCode.BORICA_GET_FILE_STATUS, LogEventLifecycle.REQUEST), "Заявка за извличане на статус по документ изпратен за подписване чрез Борика" },
    { (LogEventCode.BORICA_GET_FILE_STATUS, LogEventLifecycle.SUCCESS), "Успешно извличане на статус по документ изпратен за подписване чрез Борика" },
    { (LogEventCode.BORICA_GET_FILE_STATUS, LogEventLifecycle.FAIL), "Неуспешно извличане на статус по документ изпратен за подписване чрез Борика" },

    { (LogEventCode.BORICA_DOWNLOAD_FILE, LogEventLifecycle.REQUEST), "Заявка за сваляне на подписан документ чрез Борика" },
    { (LogEventCode.BORICA_DOWNLOAD_FILE, LogEventLifecycle.SUCCESS), "Успешно сваляне на подписан документ чрез Борика" },
    { (LogEventCode.BORICA_DOWNLOAD_FILE, LogEventLifecycle.FAIL), "Неуспешно сваляне на подписан документ чрез Борика" },

    { (LogEventCode.BORICA_CHECK_USER, LogEventLifecycle.REQUEST), "Заявка за проверка на гражданин в Борика" },
    { (LogEventCode.BORICA_CHECK_USER, LogEventLifecycle.SUCCESS), "Успешна проверка на гражданин в Борика" },
    { (LogEventCode.BORICA_CHECK_USER, LogEventLifecycle.FAIL), "Неуспешна проверка на гражданин в Борика" },

    { (LogEventCode.KEP_GET_DATA_TO_SIGN, LogEventLifecycle.REQUEST), "Заявка за подготвяне на данни за подписване с КЕП" },
    { (LogEventCode.KEP_GET_DATA_TO_SIGN, LogEventLifecycle.SUCCESS), "Успешно подготвяне на данни за подписване с КЕП" },
    { (LogEventCode.KEP_GET_DATA_TO_SIGN, LogEventLifecycle.FAIL), "Неуспешно подготвяне на данни за подписване с КЕП" },

    { (LogEventCode.KEP_SIGN_DATA, LogEventLifecycle.REQUEST), "Заявка за подписване на данни с КЕП" },
    { (LogEventCode.KEP_SIGN_DATA, LogEventLifecycle.SUCCESS), "Успешно подписване на данни с КЕП" },
    { (LogEventCode.KEP_SIGN_DATA, LogEventLifecycle.FAIL), "Неуспешно подписване на данни с КЕП" }
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
