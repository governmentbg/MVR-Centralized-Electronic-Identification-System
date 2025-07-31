namespace eID.PUN.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
    {
        { (LogEventCode.REGISTER_CARRIER, LogEventLifecycle.REQUEST), "Заявка за регистрация на носител" },
        { (LogEventCode.REGISTER_CARRIER, LogEventLifecycle.SUCCESS), "Успешна регистрация на носител" },
        { (LogEventCode.REGISTER_CARRIER, LogEventLifecycle.FAIL), "Неуспешна регистрация на носител" },

        { (LogEventCode.GET_CARRIERS_BY, LogEventLifecycle.REQUEST), "Заявка за извличане на носители" },
        { (LogEventCode.GET_CARRIERS_BY, LogEventLifecycle.SUCCESS), "Успешно извличане на носители" },
        { (LogEventCode.GET_CARRIERS_BY, LogEventLifecycle.FAIL), "Неуспешно извличане на носители" }
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
