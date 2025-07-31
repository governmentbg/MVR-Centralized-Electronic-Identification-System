namespace eID.RO.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
    {
        { (LogEventCode.CREATE_EMPOWERMENT, LogEventLifecycle.REQUEST), "Заявка за създаване на изявление за овластяване" },
        { (LogEventCode.CREATE_EMPOWERMENT, LogEventLifecycle.SUCCESS), "Успешно създаване на изявление за овластяване" },
        { (LogEventCode.CREATE_EMPOWERMENT, LogEventLifecycle.FAIL), "Неуспешно създаване на изявление за овластяване" },

        { (LogEventCode.GET_EMPOWERMENTS_TO_ME_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с овластявания в ролята на овластен" },
        { (LogEventCode.GET_EMPOWERMENTS_TO_ME_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с овластявания в ролята на овластен" },
        { (LogEventCode.GET_EMPOWERMENTS_TO_ME_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с овластявания в ролята на овластен" },

        { (LogEventCode.GET_EMPOWERMENTS_FROM_ME_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с овластявания в ролята на овластител" },
        { (LogEventCode.GET_EMPOWERMENTS_FROM_ME_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с овластявания в ролята на овластител" },
        { (LogEventCode.GET_EMPOWERMENTS_FROM_ME_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с овластявания в ролята на овластител" },

        { (LogEventCode.WITHDRAW_EMPOWERMENT, LogEventLifecycle.REQUEST), "Заявка за оттегляне на овластяване" },
        { (LogEventCode.WITHDRAW_EMPOWERMENT, LogEventLifecycle.SUCCESS), "Успешно оттегляне на овластяване" },
        { (LogEventCode.WITHDRAW_EMPOWERMENT, LogEventLifecycle.FAIL), "Неуспешно оттегляне на овластяване" },

        { (LogEventCode.DISAGREE_EMPOWERMENT, LogEventLifecycle.REQUEST), "Заявка за деклариране на несъгласие за овластяване" },
        { (LogEventCode.DISAGREE_EMPOWERMENT, LogEventLifecycle.SUCCESS), "Успешно деклариране на несъгласие за овластяване" },
        { (LogEventCode.DISAGREE_EMPOWERMENT, LogEventLifecycle.FAIL), "Неуспешно деклариране на несъгласие за овластяване" },

        { (LogEventCode.GET_EMPOWERMENTS_BY_DEAU, LogEventLifecycle.REQUEST), "Заявка за проверка на заявления от ДЕАУ" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_DEAU, LogEventLifecycle.SUCCESS), "Успешна проверка на заявления от ДЕАУ" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_DEAU, LogEventLifecycle.FAIL), "Неуспешна проверка на заявления от ДЕАУ" },

        { (LogEventCode.SIGN_EMPOWERMENT, LogEventLifecycle.REQUEST), "Заявка за подписване на овластяване" },
        { (LogEventCode.SIGN_EMPOWERMENT, LogEventLifecycle.SUCCESS), "Успешно подписване на овластяване" },
        { (LogEventCode.SIGN_EMPOWERMENT, LogEventLifecycle.FAIL), "Неуспешно подписване на овластяване" },

        { (LogEventCode.GET_EMPOWERMENTS_BY_EIK, LogEventLifecycle.REQUEST), "Заявка за извличане на овластявания по ЕИК" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_EIK, LogEventLifecycle.SUCCESS), "Успешно извличане на овластявания по ЕИК" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_EIK, LogEventLifecycle.FAIL), "Неуспешно извличане на овластявания по ЕИК" },

        { (LogEventCode.DENY_EMPOWERMENT_BY_DEAU, LogEventLifecycle.REQUEST), "Заявка за отказване на овластяване от ДЕАУ" },
        { (LogEventCode.DENY_EMPOWERMENT_BY_DEAU, LogEventLifecycle.SUCCESS), "Успешно отказване на овластяване от ДЕАУ" },
        { (LogEventCode.DENY_EMPOWERMENT_BY_DEAU, LogEventLifecycle.FAIL), "Неуспешно отказване на овластяване от ДЕАУ" },

        { (LogEventCode.APPROVE_EMPOWERMENT_BY_DEAU, LogEventLifecycle.REQUEST), "Заявка за одобрение на овластяване от ДЕАУ" },
        { (LogEventCode.APPROVE_EMPOWERMENT_BY_DEAU, LogEventLifecycle.SUCCESS), "Успешно одобрение на овластяване от ДЕАУ" },
        { (LogEventCode.APPROVE_EMPOWERMENT_BY_DEAU, LogEventLifecycle.FAIL), "Неуспешно одобрение на овластяване от ДЕАУ" },

        { (LogEventCode.GET_EMPOWERMENTS_BY_ADMINISTRATOR, LogEventLifecycle.REQUEST), "Заявка за извличане на овластявания от администратор" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_ADMINISTRATOR, LogEventLifecycle.SUCCESS), "Успешно извличане на овластявания от администратор" },
        { (LogEventCode.GET_EMPOWERMENTS_BY_ADMINISTRATOR, LogEventLifecycle.FAIL), "Неуспешно извличане на овластявания от администратор" },

        { (LogEventCode.EXPORT_EMPOWERMENT, LogEventLifecycle.REQUEST), "Заявка за генериране на удостоверение за овластяване от администратор" },
        { (LogEventCode.EXPORT_EMPOWERMENT, LogEventLifecycle.SUCCESS), "Успешно генериране на удостоверение за овластяване от администратор" },
        { (LogEventCode.EXPORT_EMPOWERMENT, LogEventLifecycle.FAIL), "Неуспешно генериране на удостоверение за овластяване от администратор" },

        { (LogEventCode.CHANGE_EMPOWERMENT_STATUS, LogEventLifecycle.REQUEST), "Заявка за смяна на статус на овластяване" },
        { (LogEventCode.CHANGE_EMPOWERMENT_STATUS, LogEventLifecycle.SUCCESS), "Успешна смяна на статус на овластяване" },
        { (LogEventCode.CHANGE_EMPOWERMENT_STATUS, LogEventLifecycle.FAIL), "Заявка за смяна на статус на овластяване" },

        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_TO_ME, LogEventLifecycle.REQUEST), "Заявка за генериране на удостоверение за овластяване в ролята на овластен" },
        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_TO_ME, LogEventLifecycle.SUCCESS), "Успешно генериране на удостоверение за овластяване в ролята на овластен" },
        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_TO_ME, LogEventLifecycle.FAIL), "Неуспешно генериране на удостоверение за овластяване в ролята на овластен" },

        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_FROM_ME, LogEventLifecycle.REQUEST), "Заявка за генериране на удостоверение за овластяване в ролята на овластител" },
        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_FROM_ME, LogEventLifecycle.SUCCESS), "Успешно генериране на удостоверение за овластяване в ролята на овластител" },
        { (LogEventCode.GET_EMPOWERMENT_DOCUMENT_FROM_ME, LogEventLifecycle.FAIL), "Неуспешно генериране на удостоверение за овластяване в ролята на овластител" },

        { (LogEventCode.EMPOWERMENT_IS_COLLECTED_SIGNATURES, LogEventLifecycle.REQUEST), "Заявка за събиране на подписи за овластяване" },
        { (LogEventCode.EMPOWERMENT_IS_COLLECTED_SIGNATURES, LogEventLifecycle.SUCCESS), "Овластяване е подписано" },
        { (LogEventCode.EMPOWERMENT_IS_COLLECTED_SIGNATURES, LogEventLifecycle.FAIL), "Неуспешно събиране на подписи за овластяване" },

        { (LogEventCode.EMPOWERMENT_IS_ACTIVATED, LogEventLifecycle.REQUEST), "Заявка за вписване на овластяване" },
        { (LogEventCode.EMPOWERMENT_IS_ACTIVATED, LogEventLifecycle.SUCCESS), "Овластяване е вписано" },
        { (LogEventCode.EMPOWERMENT_IS_ACTIVATED, LogEventLifecycle.FAIL), "Неуспешно вписване на овластяване" },

        { (LogEventCode.EMPOWERMENT_IS_DENIED, LogEventLifecycle.REQUEST), "Заявка за отказ на овластяване" },
        { (LogEventCode.EMPOWERMENT_IS_DENIED, LogEventLifecycle.SUCCESS), "Овластяване е отказано" },
        { (LogEventCode.EMPOWERMENT_IS_DENIED, LogEventLifecycle.FAIL), "Неуспешен отказ на овластяване" },

        { (LogEventCode.EMPOWERMENT_IS_DISAGREEMENT_DECLARED, LogEventLifecycle.REQUEST), "Заявка за деклариране на несъгласие с овластяване" },
        { (LogEventCode.EMPOWERMENT_IS_DISAGREEMENT_DECLARED, LogEventLifecycle.SUCCESS), "Oвластяване е с декларирано несъгласие" },
        { (LogEventCode.EMPOWERMENT_IS_DISAGREEMENT_DECLARED, LogEventLifecycle.FAIL), "Неуспешно деклариране на несъгласие с овластяване" },

        { (LogEventCode.EMPOWERMENT_IS_WITHDRAWN, LogEventLifecycle.REQUEST), "Заявка за оттегляне на овластяване" },
        { (LogEventCode.EMPOWERMENT_IS_WITHDRAWN, LogEventLifecycle.SUCCESS), "Овластяване е оттеглено" },
        { (LogEventCode.EMPOWERMENT_IS_WITHDRAWN, LogEventLifecycle.FAIL), "Неуспешно оттегляне на овластяване" },

        { (LogEventCode.EMPOWERMENT_IS_UNCONFIRMED, LogEventLifecycle.REQUEST), "Заявка за отбелязване на овластяване като непотвърдено" },
        { (LogEventCode.EMPOWERMENT_IS_UNCONFIRMED, LogEventLifecycle.SUCCESS), "Овластяване е непотвърдено" },
        { (LogEventCode.EMPOWERMENT_IS_UNCONFIRMED, LogEventLifecycle.FAIL), "Неуспешно отбелязване на овластяване като непотвърдено" },
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
