namespace eID.PDEAU.Contracts;

public static class LogEventMessages
{
    private static Dictionary<(LogEventCode code, LogEventLifecycle state), string> Messages { get; } = new()
{
    { (LogEventCode.GET_LEGAL_ENTITY_INFO, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за юридическо лице" },
    { (LogEventCode.GET_LEGAL_ENTITY_INFO, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за юридическо лице" },
    { (LogEventCode.GET_LEGAL_ENTITY_INFO, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за юридическо лице" },

    { (LogEventCode.REGISTER_PROVIDER, LogEventLifecycle.REQUEST), "Заявка за регистране на ДЕАУ" },
    { (LogEventCode.REGISTER_PROVIDER, LogEventLifecycle.SUCCESS), "Успешно регистране на ДЕАУ" },
    { (LogEventCode.REGISTER_PROVIDER, LogEventLifecycle.FAIL), "Неуспешно регистране на ДЕАУ" },

    { (LogEventCode.GET_ALL_PROVIDERS, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с регистрирани ДЕАУ" },
    { (LogEventCode.GET_ALL_PROVIDERS, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с регистрирани ДЕАУ" },
    { (LogEventCode.GET_ALL_PROVIDERS, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с регистрирани ДЕАУ" },

    { (LogEventCode.PROMOTE_USER_TO_ADMIN, LogEventLifecycle.REQUEST), "PromoteUserToAdmin_REQUEST" },
    { (LogEventCode.PROMOTE_USER_TO_ADMIN, LogEventLifecycle.SUCCESS), "PromoteUserToAdmin_SUCCESS" },
    { (LogEventCode.PROMOTE_USER_TO_ADMIN, LogEventLifecycle.FAIL), "PromoteUserToAdmin_FAIL" },

    { (LogEventCode.INITIATE_ADMIN_PROMOTION, LogEventLifecycle.REQUEST), "InitiateAdminPromotion_REQUEST" },
    { (LogEventCode.INITIATE_ADMIN_PROMOTION, LogEventLifecycle.SUCCESS), "InitiateAdminPromotion_SUCCESS" },
    { (LogEventCode.INITIATE_ADMIN_PROMOTION, LogEventLifecycle.FAIL), "InitiateAdminPromotion_FAIL" },

    { (LogEventCode.CONFIRM_ADMIN_PROMOTION, LogEventLifecycle.REQUEST), "ConfirmAdminPromotion_REQUEST" },
    { (LogEventCode.CONFIRM_ADMIN_PROMOTION, LogEventLifecycle.SUCCESS), "ConfirmAdminPromotion_SUCCESS" },
    { (LogEventCode.CONFIRM_ADMIN_PROMOTION, LogEventLifecycle.FAIL), "ConfirmAdminPromotion_FAIL" },

    { (LogEventCode.GET_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с регистрирани ДЕАУ по критерии" },
    { (LogEventCode.GET_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с регистрирани ДЕАУ по критерии" },
    { (LogEventCode.GET_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с регистрирани ДЕАУ по критерии" },

    { (LogEventCode.GET_USER_DETAILS, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за служител на ДЕАУ" },
    { (LogEventCode.GET_USER_DETAILS, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за служител на ДЕАУ" },
    { (LogEventCode.GET_USER_DETAILS, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за служител на ДЕАУ" },

    { (LogEventCode.REGISTER_USER, LogEventLifecycle.REQUEST), "Заявка за регистриране на служител на ДЕАУ" },
    { (LogEventCode.REGISTER_USER, LogEventLifecycle.SUCCESS), "Успешно регистриране на служител на ДЕАУ" },
    { (LogEventCode.REGISTER_USER, LogEventLifecycle.FAIL), "Неуспешно регистриране на служител на ДЕАУ" },

    { (LogEventCode.GET_USERS_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане служителите на ДЕАУ по критерии" },
    { (LogEventCode.GET_USERS_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане служителите на ДЕАУ по критерии" },
    { (LogEventCode.GET_USERS_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане служителите на ДЕАУ по критерии" },

    { (LogEventCode.GET_PROVIDER_DETAILS_BY_ID, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за ДЕАУ по идентификатор" },
    { (LogEventCode.GET_PROVIDER_DETAILS_BY_ID, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за ДЕАУ по идентификатор" },
    { (LogEventCode.GET_PROVIDER_DETAILS_BY_ID, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за ДЕАУ по идентификатор" },

    //{ (LogEventCode.SCRAPE_IISDA_OK, LogEventLifecycle.REQUEST), "Заявка за " },
    //{ (LogEventCode.SCRAPE_IISDA_OK, LogEventLifecycle.SUCCESS), "ScrapeIISDAOK_SUCCESS" },
    //{ (LogEventCode.SCRAPE_IISDA_OK, LogEventLifecycle.FAIL), "ScrapeIISDAOK_FAIL" },

    //{ (LogEventCode.SCRAPE_IISDA_ERROR, LogEventLifecycle.REQUEST), "ScrapeIISDAError_REQUEST" },
    //{ (LogEventCode.SCRAPE_IISDA_ERROR, LogEventLifecycle.SUCCESS), "ScrapeIISDAError_SUCCESS" },
    //{ (LogEventCode.SCRAPE_IISDA_ERROR, LogEventLifecycle.FAIL), "ScrapeIISDAError_FAIL" },

    { (LogEventCode.UPDATE_SERVICE, LogEventLifecycle.REQUEST), "Заявка за редактиране на услуга" },
    { (LogEventCode.UPDATE_SERVICE, LogEventLifecycle.SUCCESS), "Успешно редактиране на услуга" },
    { (LogEventCode.UPDATE_SERVICE, LogEventLifecycle.FAIL), "Неуспешно редактиране на услуга" },

    { (LogEventCode.SET_PROVIDER_DETAILS_STATUS, LogEventLifecycle.REQUEST), "Заявка за промяна статуса на ДЕАУ" },
    { (LogEventCode.SET_PROVIDER_DETAILS_STATUS, LogEventLifecycle.SUCCESS), "Успешна промяна статуса на ДЕАУ" },
    { (LogEventCode.SET_PROVIDER_DETAILS_STATUS, LogEventLifecycle.FAIL), "Неуспешна промяна статуса на ДЕАУ" },

    { (LogEventCode.UPDATE_USER, LogEventLifecycle.REQUEST), "Заявка за редактиране на служител на ДЕАУ" },
    { (LogEventCode.UPDATE_USER, LogEventLifecycle.SUCCESS), "Успешно редактиране на служител на ДЕАУ" },
    { (LogEventCode.UPDATE_USER, LogEventLifecycle.FAIL), "Неуспешно редактиране на служител на ДЕАУ" },

    { (LogEventCode.GET_PROVIDERS_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на ДЕАУ" },
    { (LogEventCode.GET_PROVIDERS_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на ДЕАУ" },
    { (LogEventCode.GET_PROVIDERS_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на ДЕАУ" },

    { (LogEventCode.GET_PROVIDER_FILE, LogEventLifecycle.REQUEST), "Заявка за сваляне на файл прикачен към заявление за ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_FILE, LogEventLifecycle.SUCCESS), "Успешно сваляне на файл прикачен към заявление за ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_FILE, LogEventLifecycle.FAIL), "Неуспешно сваляне на файл прикачен към заявление за ДЕАУ" },

    { (LogEventCode.GET_PROVIDERS_LIST_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на списък с ДЕАУ" },
    { (LogEventCode.GET_PROVIDERS_LIST_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на списък с ДЕАУ" },
    { (LogEventCode.GET_PROVIDERS_LIST_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на списък с ДЕАУ" },

    { (LogEventCode.GET_AVAILABLE_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.REQUEST), "Заявка за извличане на одобрени ДЕАУ" },
    { (LogEventCode.GET_AVAILABLE_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.SUCCESS), "Успешно извличане на одобрени ДЕАУ" },
    { (LogEventCode.GET_AVAILABLE_PROVIDER_DETAILS_BY_FILTER, LogEventLifecycle.FAIL), "Неуспешно извличане на одобрени ДЕАУ" },

    { (LogEventCode.APPROVE_PROVIDER, LogEventLifecycle.REQUEST), "Заявка за одобрение на заявление за регистриране на ДЕАУ" },
    { (LogEventCode.APPROVE_PROVIDER, LogEventLifecycle.SUCCESS), "Успешно одобрение на заявление за регистриране на ДЕАУ" },
    { (LogEventCode.APPROVE_PROVIDER, LogEventLifecycle.FAIL), "Неуспешно одобрение на заявление за регистриране на ДЕАУ" },

    { (LogEventCode.GET_PROVIDER_BY_ID, LogEventLifecycle.REQUEST), "Заявка за извличане на информация за ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_BY_ID, LogEventLifecycle.SUCCESS), "Успешно извличане на информация за ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_BY_ID, LogEventLifecycle.FAIL), "Неуспешно извличане на информация за ДЕАУ" },

    { (LogEventCode.RETURN_PROVIDER_FOR_CORRECTION, LogEventLifecycle.REQUEST), "Заявка за връщане на заявление за регистриране на ДЕАУ за редакция" },
    { (LogEventCode.RETURN_PROVIDER_FOR_CORRECTION, LogEventLifecycle.SUCCESS), "Успешно връщане на заявление за регистриране на ДЕАУ за редакция" },
    { (LogEventCode.RETURN_PROVIDER_FOR_CORRECTION, LogEventLifecycle.FAIL), "Неуспешно връщане на заявление за регистриране на ДЕАУ за редакция" },

    { (LogEventCode.UPDATE_PROVIDER, LogEventLifecycle.REQUEST), "Заявка за редактиране заявление за регистриране на ДЕАУ" },
    { (LogEventCode.UPDATE_PROVIDER, LogEventLifecycle.SUCCESS), "Успешно редактиране заявление за регистриране на ДЕАУ" },
    { (LogEventCode.UPDATE_PROVIDER, LogEventLifecycle.FAIL), "Неуспешно редактиране заявление за регистриране на ДЕАУ" },

    { (LogEventCode.GET_PROVIDER_STATUS_HISTORY, LogEventLifecycle.REQUEST), "Заявка за извличане историята на регистрация на ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_STATUS_HISTORY, LogEventLifecycle.SUCCESS), "Успешно извличане историята на регистрация на ДЕАУ" },
    { (LogEventCode.GET_PROVIDER_STATUS_HISTORY, LogEventLifecycle.FAIL), "Неуспешно извличане историята на регистрация на ДЕАУ" },

    { (LogEventCode.DENY_PROVIDER, LogEventLifecycle.REQUEST), "Заявка за отказване заявление за регистриране на ДЕАУ" },
    { (LogEventCode.DENY_PROVIDER, LogEventLifecycle.SUCCESS), "Успешно отказване заявление за регистриране на ДЕАУ" },
    { (LogEventCode.DENY_PROVIDER, LogEventLifecycle.FAIL), "Успешно отказване заявление за регистриране на ДЕАУ" },

    { (LogEventCode.GET_CURRENT_PROVIDER_DETAILS, LogEventLifecycle.REQUEST), "GetCurrentProviderDetails_REQUEST" },
    { (LogEventCode.GET_CURRENT_PROVIDER_DETAILS, LogEventLifecycle.SUCCESS), "GetCurrentProviderDetails_SUCCESS" },
    { (LogEventCode.GET_CURRENT_PROVIDER_DETAILS, LogEventLifecycle.FAIL), "GetCurrentProviderDetails_FAIL" },

    { (LogEventCode.REGISTER_SERVICE, LogEventLifecycle.REQUEST), "Заявление за регистриране на услуга на ДЕАУ" },
    { (LogEventCode.REGISTER_SERVICE, LogEventLifecycle.SUCCESS), "Успешно регистриране на услуга на ДЕАУ" },
    { (LogEventCode.REGISTER_SERVICE, LogEventLifecycle.FAIL), "Неуспешно регистриране на услуга на ДЕАУ" },

    { (LogEventCode.UPDATE_PROVIDER_GENERAL_INFORMATION_AND_OFFICES, LogEventLifecycle.REQUEST), "Заявка за редактиране на основната информация и офисите на ДЕАУ" },
    { (LogEventCode.UPDATE_PROVIDER_GENERAL_INFORMATION_AND_OFFICES, LogEventLifecycle.SUCCESS), "Успешно редактиране на основната информация и офисите на ДЕАУ" },
    { (LogEventCode.UPDATE_PROVIDER_GENERAL_INFORMATION_AND_OFFICES, LogEventLifecycle.FAIL), "Неуспешно редактиране на основната информация и офисите на ДЕАУ" },

    { (LogEventCode.GET_USER_BY_UID, LogEventLifecycle.REQUEST), "Заявка за извличане на служител на ДЕАУ по идентификатор" },
    { (LogEventCode.GET_USER_BY_UID, LogEventLifecycle.SUCCESS), "Успешно извличане на служител на ДЕАУ по идентификатор" },
    { (LogEventCode.GET_USER_BY_UID, LogEventLifecycle.FAIL), "Неуспешно извличане на служител на ДЕАУ по идентификатор" },

    { (LogEventCode.GET_USER_ADMINISTRATOR_ACTIONS, LogEventLifecycle.REQUEST), "Заявка за извличане действията извършени от служител на ДЕАУ " },
    { (LogEventCode.GET_USER_ADMINISTRATOR_ACTIONS, LogEventLifecycle.SUCCESS), "Успешно извличане действията извършени от служител на ДЕАУ" },
    { (LogEventCode.GET_USER_ADMINISTRATOR_ACTIONS, LogEventLifecycle.FAIL), "Неуспешно извличане действията извършени от служител на ДЕАУ" },

    { (LogEventCode.DELETE_USER, LogEventLifecycle.REQUEST), "Заявка за изтриване на служител на ДЕАУ" },
    { (LogEventCode.DELETE_USER, LogEventLifecycle.SUCCESS), "Успешно изтриване на служител на ДЕАУ" },
    { (LogEventCode.DELETE_USER, LogEventLifecycle.FAIL), "Неуспешно изтриване на служител на ДЕАУ" }
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
