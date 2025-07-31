namespace eID.RO.API.Public.Exports;

public static class StringResources
{
    private static readonly Dictionary<(LanguageType lt, ResourceType rt), string> _sr = new Dictionary<(LanguageType lt, ResourceType rt), string>
    {
        // Base statement
        { (LanguageType.Bg, ResourceType.DocumentName), "Удостоверение за овластяване" },
        { (LanguageType.En, ResourceType.DocumentName), "Certificate of empowerment" },
        
        { (LanguageType.Bg, ResourceType.TypeOfEmpowerment), "Тип овластяване" },
        { (LanguageType.En, ResourceType.TypeOfEmpowerment), "Empowerment type" },
        
        { (LanguageType.Bg, ResourceType.TogetherAdditionalText), "Oвластяването е валидно само в присъствието на всички овластени лица" },
        { (LanguageType.En, ResourceType.TogetherAdditionalText), "The empowerment is valid only in the presence of all authorized persons" },
        
        { (LanguageType.Bg, ResourceType.Together), "Само заедно" },
        { (LanguageType.En, ResourceType.Together), "Only together" },
        
        { (LanguageType.Bg, ResourceType.Reason), "Причина" },
        { (LanguageType.En, ResourceType.Reason), "Reason" },
        
        { (LanguageType.Bg, ResourceType.Indefinitely), "Безсрочно" },
        { (LanguageType.En, ResourceType.Indefinitely), "Indefinitely" },
        
        { (LanguageType.Bg, ResourceType.Number), "Номер" },
        { (LanguageType.En, ResourceType.Number), "Number" },
        
        { (LanguageType.Bg, ResourceType.Status), "Статус" },
        { (LanguageType.En, ResourceType.Status), "Status" },
        
        { (LanguageType.Bg, ResourceType.OnBehalfOf), "От името на" },
        { (LanguageType.En, ResourceType.OnBehalfOf), "On behalf of" },
        
        { (LanguageType.Bg, ResourceType.NumberOfTheApplicant), "ЕГН/ЛНЧ на овластител" },
        { (LanguageType.En, ResourceType.NumberOfTheApplicant), "EGN/LNCH number of the applicant" },
        
        { (LanguageType.Bg, ResourceType.NumberOfTheLegalEntity), "ЕИК/Булстат на юридическото лице" },
        { (LanguageType.En, ResourceType.NumberOfTheLegalEntity), "EIC/Bulstat of the legal entity" },
        
        { (LanguageType.Bg, ResourceType.Name), "Име" },
        { (LanguageType.En, ResourceType.Name), "Name" },
        
        { (LanguageType.Bg, ResourceType.LegalRepresentatives), "Законни представители" },
        { (LanguageType.En, ResourceType.LegalRepresentatives), "Legal representatives" },
        
        { (LanguageType.Bg, ResourceType.SignedOn), "Подписано на" },
        { (LanguageType.En, ResourceType.SignedOn), "Signed on" },
        
        { (LanguageType.Bg, ResourceType.EmpoweredPeople), "ЕГН/ЛНЧ на овластени лица" },
        { (LanguageType.En, ResourceType.EmpoweredPeople), "Empowered people" },
        
        { (LanguageType.Bg, ResourceType.Provider), "Доставчик" },
        { (LanguageType.En, ResourceType.Provider), "Provider" },
        
        { (LanguageType.Bg, ResourceType.Service), "Услуга" },
        { (LanguageType.En, ResourceType.Service), "Service" },
        
        { (LanguageType.Bg, ResourceType.ExtentOfRepresentativeAuthority), "Обем на представителната власт" },
        { (LanguageType.En, ResourceType.ExtentOfRepresentativeAuthority), "Extent of representative authority" },
        
        // History
        { (LanguageType.Bg, ResourceType.EmpowermentHistory), "История на овластяването" },
        { (LanguageType.En, ResourceType.EmpowermentHistory), "Empowerment history" },
        
        { (LanguageType.Bg, ResourceType.StartDate), "Валидно от" },
        { (LanguageType.En, ResourceType.StartDate), "Valid from" },
        
        { (LanguageType.Bg, ResourceType.EndDate), "Валидно до" },
        { (LanguageType.En, ResourceType.EndDate), "Valid to" },
        
        { (LanguageType.Bg, ResourceType.SubmittedTo), "Подадено на" },
        { (LanguageType.En, ResourceType.SubmittedTo), "Submitted to" },
        
        { (LanguageType.Bg, ResourceType.EffectiveOn), "Влиза в сила на" },
        { (LanguageType.En, ResourceType.EffectiveOn), "Effective on" },

        { (LanguageType.Bg, ResourceType.DeniedOn), "Отказано на" },
        { (LanguageType.En, ResourceType.DeniedOn), "Denied on" },

        { (LanguageType.Bg, ResourceType.DisagreementDeclaredOn), "Декларирано несъгласие на" },
        { (LanguageType.En, ResourceType.DisagreementDeclaredOn), "Disagreement declared on" },

        { (LanguageType.Bg, ResourceType.CollectingAuthorizerSignaturesOn), "Очаква подписване на" },
        { (LanguageType.En, ResourceType.CollectingAuthorizerSignaturesOn), "Awaiting signature on" },

        { (LanguageType.Bg, ResourceType.WithdrawnOn), "Оттеглено на" },
        { (LanguageType.En, ResourceType.WithdrawnOn), "Withdrawn on" },

        { (LanguageType.Bg, ResourceType.UnconfirmedOn), "Вписано на" },
        { (LanguageType.En, ResourceType.UnconfirmedOn), "Entered on" },
        
        // Generic
        { (LanguageType.Bg, ResourceType.EGN), "ЕГН" },
        { (LanguageType.En, ResourceType.EGN), "EGN" },
        
        { (LanguageType.Bg, ResourceType.LNCH), "ЛНЧ" },
        { (LanguageType.En, ResourceType.LNCH), "LNCH" },
        
        { (LanguageType.Bg, ResourceType.Active), "Активно" },
        { (LanguageType.En, ResourceType.Active), "Active" },
        
        { (LanguageType.Bg, ResourceType.DisagreementDeclared), "Декларирано несъгласие" },
        { (LanguageType.En, ResourceType.DisagreementDeclared), "Disagreement declared" },
        
        { (LanguageType.Bg, ResourceType.Withdrawn), "Оттеглено" },
        { (LanguageType.En, ResourceType.Withdrawn), "Withdrawn" },
        
        { (LanguageType.Bg, ResourceType.Expired), "Изтекло" },
        { (LanguageType.En, ResourceType.Expired), "Expired" },
        
        { (LanguageType.Bg, ResourceType.Unconfirmed), "Непотвърдено" },
        { (LanguageType.En, ResourceType.Unconfirmed), "Unconfirmed" },
        
        { (LanguageType.Bg, ResourceType.Individual), "Физическо лице" },
        { (LanguageType.En, ResourceType.Individual), "Individual" },
        
        { (LanguageType.Bg, ResourceType.LegalEntity), "Юридическо лице" },
        { (LanguageType.En, ResourceType.LegalEntity), "Legal entity" },
        
        { (LanguageType.Bg, ResourceType.Unknown), "Неизвестен" },
        { (LanguageType.En, ResourceType.Unknown), "Unknown" },
        
        { (LanguageType.Bg, ResourceType.CollectingAuthorizerSignatures), "Събиране на подписи" },
        { (LanguageType.En, ResourceType.CollectingAuthorizerSignatures), "Collecting signatures" },
        
        { (LanguageType.Bg, ResourceType.Denied), "Отказано" },
        { (LanguageType.En, ResourceType.Denied), "Denied" },

        { (LanguageType.Bg, ResourceType.Signed), "Подписано" },
        { (LanguageType.En, ResourceType.Signed), "Signed" },

        { (LanguageType.Bg, ResourceType.DenialReason), "Причини за отхвърляне на изявление" },
        { (LanguageType.En, ResourceType.DenialReason), "Reasons for rejection" },
        
        // Denial reasons
        { (LanguageType.Bg, ResourceType.DenialReason_DeceasedUid), "Овластител или овластено лице е починал/о" },
        { (LanguageType.En, ResourceType.DenialReason_DeceasedUid), "The applicant or empowered person is deceased." },

        { (LanguageType.Bg, ResourceType.DenialReason_ProhibitedUid), "Овластител или овластено лице е под запрещение" },
        { (LanguageType.En, ResourceType.DenialReason_ProhibitedUid), "The applicant or empowered person is under prohibition." },

        { (LanguageType.Bg, ResourceType.DenialReason_NTRCheckFailed), "Несъответствие на попълнените данни с Търговския регистър" },
        { (LanguageType.En, ResourceType.DenialReason_NTRCheckFailed), "Discrepancy in the provided data with the Commercial Register." },

        { (LanguageType.Bg, ResourceType.DenialReason_TimedOut), "Неуспешна проверка в Търговския регистър" },
        { (LanguageType.En, ResourceType.DenialReason_TimedOut), "Unsuccessful check in the Commercial Register." },

        { (LanguageType.Bg, ResourceType.DenialReason_BelowLawfulAge), "Овластител или овластено лице е под 18 години" },
        { (LanguageType.En, ResourceType.DenialReason_BelowLawfulAge), "The applicant or empowered person is under 18 years old." },

        { (LanguageType.Bg, ResourceType.DenialReason_NoPermit), "Овластител или овластено лице няма право на пребиваване" },
        { (LanguageType.En, ResourceType.DenialReason_NoPermit), "The authorized person or authorized representative does not have the right of residence." },

        { (LanguageType.Bg, ResourceType.DenialReason_LawfulAgeInfoNotAvailable), "Информация за законната възраст не е налична" },
        { (LanguageType.En, ResourceType.DenialReason_LawfulAgeInfoNotAvailable), "Information about the lawful age is not available." },

        { (LanguageType.Bg, ResourceType.DenialReason_UnsuccessfulRestrictionsCheck), "Неуспешна проверка на ограничения и запор" },
        { (LanguageType.En, ResourceType.DenialReason_UnsuccessfulRestrictionsCheck), "Unsuccessful check for restrictions and encumbrances." },

        { (LanguageType.Bg, ResourceType.DenialReason_LegalEntityNotActive), "Юридическото лице не е активно" },
        { (LanguageType.En, ResourceType.DenialReason_LegalEntityNotActive), "The legal entity is not active." },

        { (LanguageType.Bg, ResourceType.DenialReason_LegalEntityRepresentationNotMatch), "Несъответствие в законните представители" },
        { (LanguageType.En, ResourceType.DenialReason_LegalEntityRepresentationNotMatch), "Discrepancy in the legal representatives." },

        { (LanguageType.Bg, ResourceType.DenialReason_UnsuccessfulLegalEntityCheck), "Неуспешна проверка на юридическото лице" },
        { (LanguageType.En, ResourceType.DenialReason_UnsuccessfulLegalEntityCheck), "Unsuccessful check for the legal entity." },

        { (LanguageType.Bg, ResourceType.DenialReason_EmpowermentStatementNotFound), "Овластяването не е намерено" },
        { (LanguageType.En, ResourceType.DenialReason_EmpowermentStatementNotFound), "The empowerment was not found." },

        { (LanguageType.Bg, ResourceType.DenialReason_BulstatCheckFailed), "Неуспешна проверка на юридическото лице в БУЛСТАТ" },
        { (LanguageType.En, ResourceType.DenialReason_BulstatCheckFailed), "BULSTAT check failed." },

        { (LanguageType.Bg, ResourceType.DenialReason_ReregisteredInNTR), "Пререгистриран в Търговски Регистър" },
        { (LanguageType.En, ResourceType.DenialReason_ReregisteredInNTR), "Reregistered in NTR." },

        { (LanguageType.Bg, ResourceType.DenialReason_ArchivedInBulstat), "Архивиран в БУЛСТАТ" },
        { (LanguageType.En, ResourceType.DenialReason_ArchivedInBulstat), "Archived in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_InInsolvencyProceedingsInBulstat), "В производство по несъстоятелност" },
        { (LanguageType.En, ResourceType.DenialReason_InInsolvencyProceedingsInBulstat), "In insolvency proceedings in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_InsolventInBulstat), "В несъстоятелност" },
        { (LanguageType.En, ResourceType.DenialReason_InsolventInBulstat), "Insolvent in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_InLiquidationInBulstat), "В ликвидация" },
        { (LanguageType.En, ResourceType.DenialReason_InLiquidationInBulstat), "In liquidation in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_InactiveInBulstat), "Неактивен в БУЛСТАТ" },
        { (LanguageType.En, ResourceType.DenialReason_InactiveInBulstat), "Inactive in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_ClosedInBulstat), "Закрит в БУЛСТАТ" },
        { (LanguageType.En, ResourceType.DenialReason_ClosedInBulstat), "Closed in BULSTAT." },

        { (LanguageType.Bg, ResourceType.DenialReason_UnsuccessfulTimestamping), "Неуспешно полагане на времеви печат" },
        { (LanguageType.En, ResourceType.DenialReason_UnsuccessfulTimestamping), "Unsuccessful timestamping." },

        { (LanguageType.Bg, ResourceType.DenialReason_SignatureCollectionTimeOut), "Неуспешно събиране на подписи" },
        { (LanguageType.En, ResourceType.DenialReason_SignatureCollectionTimeOut), "Unsuccessful signature collection." },

        { (LanguageType.Bg, ResourceType.DenialReason_DeniedByDeauAdministrator), "Отказано от администратор на ДЕАУ" },
        { (LanguageType.En, ResourceType.DenialReason_DeniedByDeauAdministrator), "Denied by DEAU administrator." },

        { (LanguageType.Bg, ResourceType.DenialReason_InvalidUidRegistrationStatusDetected), "В списъка с овластени лица има такива с несъответстващи имена." },
        { (LanguageType.En, ResourceType.DenialReason_InvalidUidRegistrationStatusDetected), "There are individuals with mismatched names in the list of empowered persons." },

        { (LanguageType.Bg, ResourceType.DenialReason_UidsRegistrationStatusInfoNotAvailable), "Не е получена информация за профилите на овластените лица" },
        { (LanguageType.En, ResourceType.DenialReason_UidsRegistrationStatusInfoNotAvailable), "No information received for the profiles of empowered persons." },

        { (LanguageType.Bg, ResourceType.DenialReason_RegistrationStatusUnavailable), "Системна грешка." },
        { (LanguageType.En, ResourceType.DenialReason_RegistrationStatusUnavailable), "System error." },

        { (LanguageType.Bg, ResourceType.DenialReason_InactiveProfile), "В списъка с описаните лица има такива, които нямат електронна идентичност, регистриран профил в ЦСЕИ или двете не са обвързани." },
        { (LanguageType.En, ResourceType.DenialReason_InactiveProfile), "The list of described persons includes individuals who either do not have an electronic identity, a registered profile in CSEI, or the two are not linked." },

        { (LanguageType.Bg, ResourceType.DenialReason_NoBaseProfile), "В списъка с описаните лица има такива, които нямат електронна идентичност, регистриран профил в ЦСЕИ или двете не са обвързани." },
        { (LanguageType.En, ResourceType.DenialReason_NoBaseProfile), "The list of described persons includes individuals who either do not have an electronic identity, a registered profile in CSEI, or the two are not linked." },

        { (LanguageType.Bg, ResourceType.DenialReason_NameMismatch), "В списъка с описаните лица има такива с несъответстващи имена." },
        { (LanguageType.En, ResourceType.DenialReason_NameMismatch), "The list of described persons includes individuals with mismatched names." },

        { (LanguageType.Bg, ResourceType.DenialReason_NoRegistration), "В списъка с описаните лица има такива, които нямат електронна идентичност, регистриран профил в ЦСЕИ или двете не са обвързани." },
        { (LanguageType.En, ResourceType.DenialReason_NoRegistration), "The list of described persons includes individuals who either do not have an electronic identity, a registered profile in CSEI, or the two are not linked." },

    };

    public static string Get(LanguageType language, ResourceType resourceType)
    {
        if (!_sr.TryGetValue((language, resourceType), out var result))
        {
            return "###";
        }

        return result;
    }
}
