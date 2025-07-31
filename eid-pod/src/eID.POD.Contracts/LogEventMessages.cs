namespace eID.POD.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
{
    { (LogEventCode.CREATE_DATASET, LogEventLifecycle.REQUEST), "Заявка за създаване на набор от данни в портал за отворени данни" },
    { (LogEventCode.CREATE_DATASET, LogEventLifecycle.SUCCESS), "Успешно създаване на набор от данни в портал за отворени данни" },
    { (LogEventCode.CREATE_DATASET, LogEventLifecycle.FAIL), "Неуспешно създаване на набор от данни в портал за отворени данни" },

    { (LogEventCode.UPDATE_DATASET, LogEventLifecycle.REQUEST), "Заявка за промяна на набор от данни в портал за отворени данни" },
    { (LogEventCode.UPDATE_DATASET, LogEventLifecycle.SUCCESS), "Успешна промяна на набор от данни в портал за отворени данни" },
    { (LogEventCode.UPDATE_DATASET, LogEventLifecycle.FAIL), "Неуспешна промяна на набор от данни в портал за отворени данни" },

    { (LogEventCode.DELETE_DATASET, LogEventLifecycle.REQUEST), "Заявка за изтриване на набор от данни в портал за отворени данни" },
    { (LogEventCode.DELETE_DATASET, LogEventLifecycle.SUCCESS), "Успешно изтриване на набор от данни в портал за отворени данни" },
    { (LogEventCode.DELETE_DATASET, LogEventLifecycle.FAIL), "Неуспешно изтриване на набор от данни в портал за отворени данни" },

    { (LogEventCode.MANUAL_UPLOAD_DATASET, LogEventLifecycle.REQUEST), "Заявка за ръчно публикуване на набор от данни в портал за отворени данни" },
    { (LogEventCode.MANUAL_UPLOAD_DATASET, LogEventLifecycle.SUCCESS), "Успешно ръчно публикуване на набор от данни в портал за отворени данни" },
    { (LogEventCode.MANUAL_UPLOAD_DATASET, LogEventLifecycle.FAIL), "Неуспешно ръчно публикуване на набор от данни в портал за отворени данни" },

    { (LogEventCode.GET_ALL_DATASETS, LogEventLifecycle.REQUEST), "Заявка за извличане на списък със набор от данни от портал за отворени данни" },
    { (LogEventCode.GET_ALL_DATASETS, LogEventLifecycle.SUCCESS), "Успешно извличане на списък със набор от данни от портал за отворени данни" },
    { (LogEventCode.GET_ALL_DATASETS, LogEventLifecycle.FAIL), "Неуспешно извличане на списък със набор от данни от портал за отворени данни" }
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
