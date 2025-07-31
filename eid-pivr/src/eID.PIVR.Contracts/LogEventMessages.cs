namespace eID.PIVR.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
    {
        { (LogEventCode.REGIX_SEARCH, LogEventLifecycle.REQUEST), "Заявка за извличане на данни от Regix" },
        { (LogEventCode.REGIX_SEARCH, LogEventLifecycle.SUCCESS), "Успешно извличане на данни от Regix" },
        { (LogEventCode.REGIX_SEARCH, LogEventLifecycle.FAIL), "Неуспешно извличане на данни от Regix" },

        { (LogEventCode.GET_DATE_OF_DEATH, LogEventLifecycle.REQUEST), "Заявка за извличане дата на смърт" },
        { (LogEventCode.GET_DATE_OF_DEATH, LogEventLifecycle.SUCCESS), "Успешно извличане дата на смърт" },
        { (LogEventCode.GET_DATE_OF_DEATH, LogEventLifecycle.FAIL), "Неуспешно извличане дата на смърт" },

        { (LogEventCode.GET_DATE_OF_PROHIBITION, LogEventLifecycle.REQUEST), "Заявка за извличане дата на запрещение" },
        { (LogEventCode.GET_DATE_OF_PROHIBITION, LogEventLifecycle.SUCCESS), "Успешно извличане дата на запрещение" },
        { (LogEventCode.GET_DATE_OF_PROHIBITION, LogEventLifecycle.FAIL), "Неуспешно извличане дата на запрещение" },

        { (LogEventCode.TR_GET_ACTUAL_STATE_V3, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за ЮЛ от ТР през Regix" },
        { (LogEventCode.TR_GET_ACTUAL_STATE_V3, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за ЮЛ от ТР през Regix" },
        { (LogEventCode.TR_GET_ACTUAL_STATE_V3, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за ЮЛ от ТР през Regix" },

        { (LogEventCode.VERIFY_SIGNATURE, LogEventLifecycle.REQUEST), "Заявка за валидиране на електронно подписано съдържание" },
        { (LogEventCode.VERIFY_SIGNATURE, LogEventLifecycle.SUCCESS), "Успешно валидиране на електронно подписано съдържание" },
        { (LogEventCode.VERIFY_SIGNATURE, LogEventLifecycle.FAIL), "Неуспешно валидиране на електронно подписано съдържание" },

        { (LogEventCode.MVR_GET_FOREIGN_IDENTITY_V2, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за чуждестранен гражданин" },
        { (LogEventCode.MVR_GET_FOREIGN_IDENTITY_V2, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за чуждестранен гражданин" },
        { (LogEventCode.MVR_GET_FOREIGN_IDENTITY_V2, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за чуждестранен гражданин" },

        { (LogEventCode.MVR_GET_PERSONAL_IDENTITY_V2, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за български гражданин" },
        { (LogEventCode.MVR_GET_PERSONAL_IDENTITY_V2, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за български гражданин" },
        { (LogEventCode.MVR_GET_PERSONAL_IDENTITY_V2, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за български гражданин" },

        { (LogEventCode.GRAO_RELATIONS_SEARCH, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за родственост от ГРАО през RegiX" },
        { (LogEventCode.GRAO_RELATIONS_SEARCH, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за родственост от ГРАО през RegiX" },
        { (LogEventCode.GRAO_RELATIONS_SEARCH, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за родственост от ГРАО през RegiX" },

        { (LogEventCode.GET_DECEASED_BY_PERIOD, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с починали лица за период" },
        { (LogEventCode.GET_DECEASED_BY_PERIOD, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с починали лица за период" },
        { (LogEventCode.GET_DECEASED_BY_PERIOD, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с починали лица за период" },

        { (LogEventCode.BULSTAT_GET_STATE_OF_PLAY, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за ЮЛ от регистър Булстат" },
        { (LogEventCode.BULSTAT_GET_STATE_OF_PLAY, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за ЮЛ от регистър Булстат" },
        { (LogEventCode.BULSTAT_GET_STATE_OF_PLAY, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за ЮЛ от регистър Булстат" },

        { (LogEventCode.GET_ID_CHANGES, LogEventLifecycle.REQUEST), "Заявка за извличане на промени по граждански идентификатор" },
        { (LogEventCode.GET_ID_CHANGES, LogEventLifecycle.SUCCESS), "Успешно извличане на промени по граждански идентификатор" },
        { (LogEventCode.GET_ID_CHANGES, LogEventLifecycle.FAIL), "Неуспешно извличане на промени по граждански идентификатор" },

        { (LogEventCode.GET_STATUT_CHANGES, LogEventLifecycle.REQUEST), "Заявка за извличане статута на гражданин по идентификатор" },
        { (LogEventCode.GET_STATUT_CHANGES, LogEventLifecycle.SUCCESS), "Успешно извличане статута на гражданин по идентификатор" },
        { (LogEventCode.GET_STATUT_CHANGES, LogEventLifecycle.FAIL), "Неуспешно извличане статута на гражданин по идентификатор" },

        { (LogEventCode.CHECK_UID_RESTRICTIONS, LogEventLifecycle.REQUEST), "Заявка за извличане дата на смърт и дата на запрещение" },
        { (LogEventCode.CHECK_UID_RESTRICTIONS, LogEventLifecycle.SUCCESS), "Успешно извличане на дата на смърт и дата на запрещение" },
        { (LogEventCode.CHECK_UID_RESTRICTIONS, LogEventLifecycle.FAIL), "Неуспешно извличане на дата на смърт и дата на запрещение" }
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
