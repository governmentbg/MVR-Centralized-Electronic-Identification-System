/**
 * single<Class>(named("name")){Class()} -> for creating a specific instance in module
 * single<Class1>{Class1(get<Class2>(named("name")))} -> for creating a specific instance in module
 * val nameOfVariable: Class by inject(named("name")) -> for creating a specific instance in class
 * get<Class>{parametersOf("param")} -> parameter passing in module
 * single<Class>{(param: String)->Class(param)} -> parameter passing in module
 * val name: Class by inject(parameters={parametersOf("param")}) -> parameter passing in class
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.domain.di

import com.digitall.eid.domain.usecase.administrators.GetAdministratorFrontOfficesUseCase
import com.digitall.eid.domain.usecase.administrators.GetAdministratorsUseCase
import com.digitall.eid.domain.usecase.applications.all.CompleteApplicationUseCase
import com.digitall.eid.domain.usecase.applications.all.GetApplicationDetailsUseCase
import com.digitall.eid.domain.usecase.applications.all.GetApplicationsUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateConfirmWithBaseProfileUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateConfirmWithEIDUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateEnrollWithBaseProfileUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateEnrollWithEIDUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateGetInitialInformationUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateSignWithBaseProfileUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithBoricaUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithEvrotrustUseCase
import com.digitall.eid.domain.usecase.applications.create.ApplicationSignWithoutProviderUseCase
import com.digitall.eid.domain.usecase.applications.create.GetApplicationUserDetailsUseCase
import com.digitall.eid.domain.usecase.assets.localization.GetAssetsLocalizationsUseCase
import com.digitall.eid.domain.usecase.authentication.AuthenticationGenerateChallengeUseCase
import com.digitall.eid.domain.usecase.authentication.AuthenticationWithBasicProfileUseCase
import com.digitall.eid.domain.usecase.authentication.AuthenticationWithCertificateUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateApplicationSignWithBoricaUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateApplicationSignWithEvrotrustUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateChangeStatusInformationUseCase
import com.digitall.eid.domain.usecase.certificates.CertificateChangeStatusUseCase
import com.digitall.eid.domain.usecase.certificates.GetCertificateDetailsUseCase
import com.digitall.eid.domain.usecase.certificates.GetCertificatesUseCase
import com.digitall.eid.domain.usecase.citizen.eid.associate.CitizenEidAssociateUseCase
import com.digitall.eid.domain.usecase.certificates.SetCertificateAliasUseCase
import com.digitall.eid.domain.usecase.citizen.forgotten.password.CitizenForgottenPasswordUseCase
import com.digitall.eid.domain.usecase.citizen.registration.RegistrationRegisterNewCitizenUseCase
import com.digitall.eid.domain.usecase.citizen.update.email.UpdateCitizenEmailUseCase
import com.digitall.eid.domain.usecase.citizen.update.password.UpdateCitizenPasswordUseCase
import com.digitall.eid.domain.usecase.citizen.update.information.UpdateCitizenInformationUseCase
import com.digitall.eid.domain.usecase.devices.GetDevicesUseCase
import com.digitall.eid.domain.usecase.empowerment.cancel.CancelEmpowermentFromMeUseCase
import com.digitall.eid.domain.usecase.empowerment.cancel.CancelEmpowermentToMeUseCase
import com.digitall.eid.domain.usecase.empowerment.cancel.GetEmpowermentFromMeCancelReasonsUseCase
import com.digitall.eid.domain.usecase.empowerment.cancel.GetEmpowermentToMeCancelReasonsUseCase
import com.digitall.eid.domain.usecase.empowerment.create.CreateEmpowermentUseCase
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentProvidersUseCase
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentServiceScopeUseCase
import com.digitall.eid.domain.usecase.empowerment.create.GetEmpowermentServicesUseCase
import com.digitall.eid.domain.usecase.empowerment.from.me.GetEmpowermentFromMeUseCase
import com.digitall.eid.domain.usecase.empowerment.legal.GetEmpowermentLegalUseCase
import com.digitall.eid.domain.usecase.empowerment.signing.EmpowermentSignWithBoricaUseCase
import com.digitall.eid.domain.usecase.empowerment.signing.EmpowermentSignWithEvrotrustUseCase
import com.digitall.eid.domain.usecase.empowerment.to.me.GetEmpowermentToMeUseCase
import com.digitall.eid.domain.usecase.events.LogEventUseCase
import com.digitall.eid.domain.usecase.journal.GetJournalFromMeUseCase
import com.digitall.eid.domain.usecase.journal.GetJournalToMeUseCase
import com.digitall.eid.domain.usecase.logout.LogoutUseCase
import com.digitall.eid.domain.usecase.mfa.MfaGenerateNewOtpCodeUseCase
import com.digitall.eid.domain.usecase.mfa.MfaVerifyOtpCodeUseCase
import com.digitall.eid.domain.usecase.nomenclatures.NomenclaturesGetReasonsUseCase
import com.digitall.eid.domain.usecase.notifications.channels.GetNotificationChannelsUseCase
import com.digitall.eid.domain.usecase.notifications.channels.SetSelectedNotificationChannelsUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.GetNotificationsUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.ReverseNotificationOpenStateUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SetNotSelectedNotificationUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SetNotSelectedParentNotificationUseCase
import com.digitall.eid.domain.usecase.notifications.notifications.SubscribeToNotificationsUseCase
import com.digitall.eid.domain.usecase.payments.history.GetPaymentsHistoryUseCase
import com.digitall.eid.domain.usecase.requests.GetAllRequestsUseCase
import com.digitall.eid.domain.usecase.requests.SetOutcomeRequestUseCase
import com.digitall.eid.domain.usecase.verify.login.VerifyLoginUseCase
import org.koin.dsl.module

val useCaseModule = module {

    // auth

    factory<AuthenticationWithBasicProfileUseCase> {
        AuthenticationWithBasicProfileUseCase()
    }

    factory<AuthenticationGenerateChallengeUseCase> {
        AuthenticationGenerateChallengeUseCase()
    }

    factory<AuthenticationWithCertificateUseCase> {
        AuthenticationWithCertificateUseCase()
    }

    // notifications

    factory<GetNotificationChannelsUseCase> {
        GetNotificationChannelsUseCase()
    }

    factory<GetNotificationsUseCase> {
        GetNotificationsUseCase()
    }

    factory<SetNotSelectedNotificationUseCase> {
        SetNotSelectedNotificationUseCase()
    }

    factory<SetNotSelectedParentNotificationUseCase> {
        SetNotSelectedParentNotificationUseCase()
    }

    factory<SubscribeToNotificationsUseCase> {
        SubscribeToNotificationsUseCase()
    }

    factory<ReverseNotificationOpenStateUseCase> {
        ReverseNotificationOpenStateUseCase()
    }

    factory<SetSelectedNotificationChannelsUseCase> {
        SetSelectedNotificationChannelsUseCase()
    }

    // empowerment

    factory<GetEmpowermentFromMeUseCase> {
        GetEmpowermentFromMeUseCase()
    }

    factory<GetEmpowermentToMeUseCase> {
        GetEmpowermentToMeUseCase()
    }

    factory<GetEmpowermentServicesUseCase> {
        GetEmpowermentServicesUseCase()
    }

    factory<GetEmpowermentProvidersUseCase> {
        GetEmpowermentProvidersUseCase()
    }

    factory<CreateEmpowermentUseCase> {
        CreateEmpowermentUseCase()
    }

    factory<GetEmpowermentServiceScopeUseCase> {
        GetEmpowermentServiceScopeUseCase()
    }

    factory<GetEmpowermentToMeCancelReasonsUseCase> {
        GetEmpowermentToMeCancelReasonsUseCase()
    }

    factory<GetEmpowermentFromMeCancelReasonsUseCase> {
        GetEmpowermentFromMeCancelReasonsUseCase()
    }

    factory<CancelEmpowermentToMeUseCase> {
        CancelEmpowermentToMeUseCase()
    }

    factory<CancelEmpowermentFromMeUseCase> {
        CancelEmpowermentFromMeUseCase()
    }

    factory<EmpowermentSignWithEvrotrustUseCase> {
        EmpowermentSignWithEvrotrustUseCase()
    }

    factory<EmpowermentSignWithBoricaUseCase> {
        EmpowermentSignWithBoricaUseCase()
    }

    factory<GetEmpowermentLegalUseCase> {
        GetEmpowermentLegalUseCase()
    }

    // Journal

    factory<GetJournalFromMeUseCase> {
        GetJournalFromMeUseCase()
    }

    factory<GetJournalToMeUseCase> {
        GetJournalToMeUseCase()
    }

    // applications

    factory<GetApplicationsUseCase> {
        GetApplicationsUseCase()
    }

    factory<GetApplicationDetailsUseCase> {
        GetApplicationDetailsUseCase()
    }

    factory<GetApplicationUserDetailsUseCase> {
        GetApplicationUserDetailsUseCase()
    }

    factory<ApplicationSignWithEvrotrustUseCase> {
        ApplicationSignWithEvrotrustUseCase()
    }

    factory<ApplicationSignWithBoricaUseCase> {
        ApplicationSignWithBoricaUseCase()
    }

    factory<ApplicationCreateConfirmWithBaseProfileUseCase> {
        ApplicationCreateConfirmWithBaseProfileUseCase()
    }

    factory<ApplicationCreateEnrollWithEIDUseCase> {
        ApplicationCreateEnrollWithEIDUseCase()
    }

    factory<ApplicationCreateConfirmWithEIDUseCase> {
        ApplicationCreateConfirmWithEIDUseCase()
    }

    factory<ApplicationCreateEnrollWithBaseProfileUseCase> {
        ApplicationCreateEnrollWithBaseProfileUseCase()
    }

    factory<ApplicationCreateSignWithBaseProfileUseCase> {
        ApplicationCreateSignWithBaseProfileUseCase()
    }

    factory<CompleteApplicationUseCase> {
        CompleteApplicationUseCase()
    }

    factory<ApplicationCreateGetInitialInformationUseCase> {
        ApplicationCreateGetInitialInformationUseCase()
    }

    factory<ApplicationSignWithoutProviderUseCase> {
        ApplicationSignWithoutProviderUseCase()
    }

    // certificates

    factory<GetCertificatesUseCase> {
        GetCertificatesUseCase()
    }

    factory<GetCertificateDetailsUseCase> {
        GetCertificateDetailsUseCase()
    }

    factory<SetCertificateAliasUseCase> {
        SetCertificateAliasUseCase()
    }

    factory<CertificateApplicationSignWithEvrotrustUseCase> {
        CertificateApplicationSignWithEvrotrustUseCase()
    }

    factory<CertificateApplicationSignWithBoricaUseCase> {
        CertificateApplicationSignWithBoricaUseCase()
    }

    // registration

    factory<RegistrationRegisterNewCitizenUseCase> {
        RegistrationRegisterNewCitizenUseCase()
    }

    // update

    factory<UpdateCitizenEmailUseCase> {
        UpdateCitizenEmailUseCase()
    }

    factory<UpdateCitizenPasswordUseCase> {
        UpdateCitizenPasswordUseCase()
    }

    // forgotten

    factory<CitizenForgottenPasswordUseCase> {
        CitizenForgottenPasswordUseCase()
    }

    factory<CertificateChangeStatusUseCase> {
        CertificateChangeStatusUseCase()
    }

    factory<CertificateChangeStatusInformationUseCase> {
        CertificateChangeStatusInformationUseCase()
    }

    // nomenclatures

    factory<NomenclaturesGetReasonsUseCase> {
        NomenclaturesGetReasonsUseCase()
    }

    // registration

    factory<RegistrationRegisterNewCitizenUseCase> {
        RegistrationRegisterNewCitizenUseCase()
    }

    // update

    factory<UpdateCitizenEmailUseCase> {
        UpdateCitizenEmailUseCase()
    }

    factory<UpdateCitizenPasswordUseCase> {
        UpdateCitizenPasswordUseCase()
    }

    factory<UpdateCitizenInformationUseCase> {
        UpdateCitizenInformationUseCase()
    }

    // logout

    single<LogoutUseCase> {
        LogoutUseCase()
    }

    // verify login

    single<VerifyLoginUseCase> {
        VerifyLoginUseCase()
    }

    // devices

    single<GetDevicesUseCase> {
        GetDevicesUseCase()
    }

    // administrators

    factory<GetAdministratorsUseCase> {
        GetAdministratorsUseCase()
    }

    factory<GetAdministratorFrontOfficesUseCase> {
        GetAdministratorFrontOfficesUseCase()
    }

    // requests

    factory<GetAllRequestsUseCase> {
        GetAllRequestsUseCase()
    }

    factory<SetOutcomeRequestUseCase> {
        SetOutcomeRequestUseCase()
    }

    // assets

    factory<GetAssetsLocalizationsUseCase> {
        GetAssetsLocalizationsUseCase()
    }

    // events

    factory<LogEventUseCase> {
        LogEventUseCase()
    }

    // payments

    factory<GetPaymentsHistoryUseCase> {
        GetPaymentsHistoryUseCase()
    }

    // Associate eid

    factory<CitizenEidAssociateUseCase> {
        CitizenEidAssociateUseCase()
    }

    // MFA

    factory<MfaVerifyOtpCodeUseCase> {
        MfaVerifyOtpCodeUseCase()
    }

    factory<MfaGenerateNewOtpCodeUseCase> {
        MfaGenerateNewOtpCodeUseCase()
    }

}