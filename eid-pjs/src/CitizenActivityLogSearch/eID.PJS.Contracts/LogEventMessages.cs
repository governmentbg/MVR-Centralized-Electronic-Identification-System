namespace eID.PJS.Contracts;
public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
    {
        { (LogEventCode.GET_LOG_USER_FROM_ME, LogEventLifecycle.REQUEST), "Заявка за извличане на история на събитията" },
        { (LogEventCode.GET_LOG_USER_FROM_ME, LogEventLifecycle.SUCCESS), "Успешно извличане на история на събитията" },
        { (LogEventCode.GET_LOG_USER_FROM_ME, LogEventLifecycle.FAIL), "Неуспешно извличане на история на събитията" },

        { (LogEventCode.GET_LOG_USER_TO_ME, LogEventLifecycle.REQUEST), "Заявка за извличане на действия спрямо данни на гражданин" },
        { (LogEventCode.GET_LOG_USER_TO_ME, LogEventLifecycle.SUCCESS), "Успешно извличане на действия спрямо данни на гражданин" },
        { (LogEventCode.GET_LOG_USER_TO_ME, LogEventLifecycle.FAIL), "Неуспешно извличане на действия спрямо данни на гражданин" },

        { (LogEventCode.GET_LOG_DEAU, LogEventLifecycle.REQUEST), "Заявка за извличане на действия извършени от ДЕАУ" },
        { (LogEventCode.GET_LOG_DEAU, LogEventLifecycle.SUCCESS), "Успешно извличане на действия извършени от ДЕАУ" },
        { (LogEventCode.GET_LOG_DEAU, LogEventLifecycle.FAIL), "Неуспешно извличане на действия извършени от ДЕАУ" },

        { (LogEventCode.GET_LOG_TO_USER, LogEventLifecycle.REQUEST), "Заявка за извличане и декриптиране на действия извършени от МВР служител" },
        { (LogEventCode.GET_LOG_TO_USER, LogEventLifecycle.SUCCESS), "Успешно извличане и декриптиране на действия извършени от МВР служител" },
        { (LogEventCode.GET_LOG_TO_USER, LogEventLifecycle.FAIL), "Неуспешно извличане и декриптиране на действия извършени от МВР служител" },

        { (LogEventCode.GET_LOG_FROM_USER, LogEventLifecycle.REQUEST), "Заявка за извличане и декриптиране на действия от МВР администратор" },
        { (LogEventCode.GET_LOG_FROM_USER, LogEventLifecycle.SUCCESS), "Успешно извличане и декриптиране на действия от МВР администратор" },
        { (LogEventCode.GET_LOG_FROM_USER, LogEventLifecycle.FAIL), "Неуспешно извличане и декриптиране на действия от МВР администратор" }
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


