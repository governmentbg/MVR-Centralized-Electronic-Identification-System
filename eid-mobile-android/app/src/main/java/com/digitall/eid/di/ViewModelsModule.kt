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
package com.digitall.eid.di

import com.digitall.eid.ui.activity.MainViewModel
import com.digitall.eid.ui.fragments.administrators.AdministratorsViewModel
import com.digitall.eid.ui.fragments.administrators.flow.AdministratorsFlowViewModel
import com.digitall.eid.ui.fragments.applications.confirm.flow.ApplicationConfirmFlowViewModel
import com.digitall.eid.ui.fragments.applications.confirm.intro.ApplicationConfirmIntroViewModel
import com.digitall.eid.ui.fragments.applications.confirm.pin.ApplicationConfirmPinBottomSheetViewModel
import com.digitall.eid.ui.fragments.applications.continuecreation.continuecreation.ApplicationContinueCreationViewModel
import com.digitall.eid.ui.fragments.applications.continuecreation.flow.ApplicationContinueCreationFlowViewModel
import com.digitall.eid.ui.fragments.applications.continuecreation.pin.ApplicationContinueCreationCreatePinViewModel
import com.digitall.eid.ui.fragments.applications.create.flow.ApplicationCreateFlowViewModel
import com.digitall.eid.ui.fragments.applications.create.intro.ApplicationCreateIntroViewModel
import com.digitall.eid.ui.fragments.applications.create.pin.ApplicationCreatePinViewModel
import com.digitall.eid.ui.fragments.applications.create.preview.ApplicationCreatePreviewViewModel
import com.digitall.eid.ui.fragments.applications.payment.ApplicationPaymentViewModel
import com.digitall.eid.ui.fragments.applications.show.all.ApplicationsViewModel
import com.digitall.eid.ui.fragments.applications.show.details.ApplicationDetailsViewModel
import com.digitall.eid.ui.fragments.applications.show.filter.ApplicationFilterViewModel
import com.digitall.eid.ui.fragments.applications.show.flow.ApplicationsFlowViewModel
import com.digitall.eid.ui.fragments.auth.biometric.AuthEnterBiometricViewModel
import com.digitall.eid.ui.fragments.auth.flow.AuthFlowViewModel
import com.digitall.eid.ui.fragments.auth.mfa.AuthMfaViewModel
import com.digitall.eid.ui.fragments.auth.password.enter.AuthEnterEmailPasswordViewModel
import com.digitall.eid.ui.fragments.auth.password.forgotten.AuthForgottenPasswordViewModel
import com.digitall.eid.ui.fragments.auth.start.AuthStartViewModel
import com.digitall.eid.ui.fragments.card.enter.pin.auth.AuthCardBottomSheetViewModel
import com.digitall.eid.ui.fragments.card.enter.pin.login.CardEnterPinViewModel
import com.digitall.eid.ui.fragments.card.enter.pin.login.flow.CardEnterPinFlowViewModel
import com.digitall.eid.ui.fragments.card.scan.ScanCardBottomSheetViewModel
import com.digitall.eid.ui.fragments.centers.certification.services.CentersCertificationServicesViewModel
import com.digitall.eid.ui.fragments.centers.certification.services.flow.CentersCertificationServicesFlowViewModel
import com.digitall.eid.ui.fragments.certificates.all.CertificatesViewModel
import com.digitall.eid.ui.fragments.certificates.change.pin.CertificateChangePinViewModel
import com.digitall.eid.ui.fragments.certificates.change.pin.flow.CertificateChangePinFlowViewModel
import com.digitall.eid.ui.fragments.certificates.details.CertificateDetailsViewModel
import com.digitall.eid.ui.fragments.certificates.edit.alias.CertificateEditAliasViewModel
import com.digitall.eid.ui.fragments.certificates.enter.pin.CertificateEnterPinViewModel
import com.digitall.eid.ui.fragments.certificates.enter.pin.flow.CertificateEnterPinFlowViewModel
import com.digitall.eid.ui.fragments.certificates.filter.CertificateFilterViewModel
import com.digitall.eid.ui.fragments.certificates.flow.CertificatesFlowViewModel
import com.digitall.eid.ui.fragments.certificates.resume.CertificateResumeViewModel
import com.digitall.eid.ui.fragments.certificates.revoke.CertificateRevokeViewModel
import com.digitall.eid.ui.fragments.certificates.stop.CertificateStopViewModel
import com.digitall.eid.ui.fragments.citizen.change.email.ChangeCitizenEmailViewModel
import com.digitall.eid.ui.fragments.citizen.change.email.flow.ChangeCitizenEmailFlowViewModel
import com.digitall.eid.ui.fragments.citizen.change.information.ChangeCitizenInformationViewModel
import com.digitall.eid.ui.fragments.citizen.change.information.flow.ChangeCitizenInformationFlowViewModel
import com.digitall.eid.ui.fragments.citizen.change.password.ChangeCitizenPasswordViewModel
import com.digitall.eid.ui.fragments.citizen.change.password.flow.ChangeCitizenPasswordFlowViewModel
import com.digitall.eid.ui.fragments.citizen.information.CitizenInformationViewModel
import com.digitall.eid.ui.fragments.citizen.information.flow.CitizenInformationFlowViewModel
import com.digitall.eid.ui.fragments.citizen.profile.security.CitizenProfileSecurityViewModel
import com.digitall.eid.ui.fragments.citizen.profile.security.flow.CitizenProfileSecurityFlowViewModel
import com.digitall.eid.ui.fragments.common.search.multiselect.CommonBottomSheetWithSearchMultiselectViewModel
import com.digitall.eid.ui.fragments.common.search.normal.CommonBottomSheetWithSearchViewModel
import com.digitall.eid.ui.fragments.contacts.ContactsViewModel
import com.digitall.eid.ui.fragments.contacts.flow.ContactsFlowViewModel
import com.digitall.eid.ui.fragments.electronic.delivery.system.ElectronicDeliverySystemViewModel
import com.digitall.eid.ui.fragments.electronic.delivery.system.flow.ElectronicDeliverySystemFlowViewModel
import com.digitall.eid.ui.fragments.empowerment.all.flow.EmpowermentFlowViewModel
import com.digitall.eid.ui.fragments.empowerment.all.intro.EmpowermentIntroViewModel
import com.digitall.eid.ui.fragments.empowerment.create.create.EmpowermentCreateViewModel
import com.digitall.eid.ui.fragments.empowerment.create.create.indefinite.EmpowermentCreateIndefiniteBottomSheetViewModel
import com.digitall.eid.ui.fragments.empowerment.create.flow.EmpowermentCreateFlowViewModel
import com.digitall.eid.ui.fragments.empowerment.create.preview.EmpowermentCreatePreviewViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.all.EmpowermentFromMeViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.cancel.EmpowermentFromMeCancelViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.cancel.withdrawal.EmpowermentFromMeCancelWithdrawalBottomSheetViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.details.EmpowermentFromMeDetailsViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.filter.EmpowermentFromMeFilterViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.flow.EmpowermentFromMeFlowViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.signing.EmpowermentFromMeSigningViewModel
import com.digitall.eid.ui.fragments.empowerment.legal.all.EmpowermentLegalViewModel
import com.digitall.eid.ui.fragments.empowerment.legal.filter.EmpowermentLegalFilterViewModel
import com.digitall.eid.ui.fragments.empowerment.legal.flow.EmpowermentLegalFlowViewModel
import com.digitall.eid.ui.fragments.empowerment.legal.search.EmpowermentLegalSearchViewModel
import com.digitall.eid.ui.fragments.empowerment.to.me.all.EmpowermentToMeViewModel
import com.digitall.eid.ui.fragments.empowerment.to.me.cancel.EmpowermentToMeCancelViewModel
import com.digitall.eid.ui.fragments.empowerment.to.me.details.EmpowermentToMeDetailsViewModel
import com.digitall.eid.ui.fragments.empowerment.to.me.filter.EmpowermentToMeFilterViewModel
import com.digitall.eid.ui.fragments.empowerment.to.me.flow.EmpowermentToMeFlowViewModel
import com.digitall.eid.ui.fragments.error.biometric.BiometricErrorBottomSheetViewModel
import com.digitall.eid.ui.fragments.faq.FaqViewModel
import com.digitall.eid.ui.fragments.faq.flow.FaqFlowViewModel
import com.digitall.eid.ui.fragments.information.InformationBottomSheetViewModel
import com.digitall.eid.ui.fragments.journal.flow.JournalFlowViewModel
import com.digitall.eid.ui.fragments.journal.from.me.all.JournalFromMeViewModel
import com.digitall.eid.ui.fragments.journal.from.me.filter.JournalFromMeFilterViewModel
import com.digitall.eid.ui.fragments.journal.from.me.flow.JournalFromMeFlowViewModel
import com.digitall.eid.ui.fragments.journal.intro.JournalIntroViewModel
import com.digitall.eid.ui.fragments.journal.to.me.all.JournalToMeViewModel
import com.digitall.eid.ui.fragments.journal.to.me.filter.JournalToMeFilterViewModel
import com.digitall.eid.ui.fragments.journal.to.me.flow.JournalToMeFlowViewModel
import com.digitall.eid.ui.fragments.main.flow.MainTabsFlowViewModel
import com.digitall.eid.ui.fragments.main.tabs.eim.MainTabEIMViewModel
import com.digitall.eid.ui.fragments.main.tabs.home.MainTabHomeViewModel
import com.digitall.eid.ui.fragments.main.tabs.more.MainTabMoreViewModel
import com.digitall.eid.ui.fragments.main.tabs.requests.MainTabRequestsViewModel
import com.digitall.eid.ui.fragments.notifications.channels.NotificationChannelsViewModel
import com.digitall.eid.ui.fragments.notifications.flow.NotificationsFlowViewModel
import com.digitall.eid.ui.fragments.notifications.notifications.NotificationsViewModel
import com.digitall.eid.ui.fragments.notifications.pager.NotificationsPagerViewModel
import com.digitall.eid.ui.fragments.onlinehelpsystem.OnlineHelpSystemViewModel
import com.digitall.eid.ui.fragments.onlinehelpsystem.flow.OnlineHelpSystemFlowViewModel
import com.digitall.eid.ui.fragments.payments.filter.PaymentsHistoryFilterViewModel
import com.digitall.eid.ui.fragments.payments.history.PaymentsHistoryViewModel
import com.digitall.eid.ui.fragments.payments.history.flow.PaymentsHistoryFlowViewModel
import com.digitall.eid.ui.fragments.permissions.PermissionBottomSheetViewModel
import com.digitall.eid.ui.fragments.pin.citizen.profile.create.CreatePinCitizenProfileBottomSheetViewModel
import com.digitall.eid.ui.fragments.pin.citizen.profile.enter.EnterPinCitizenProfileBottomSheetViewModel
import com.digitall.eid.ui.fragments.providers.electronic.administrative.services.ProvidersElectronicAdministrativeServicesViewModel
import com.digitall.eid.ui.fragments.providers.electronic.administrative.services.flow.ProvidersElectronicAdministrativeServicesFlowViewModel
import com.digitall.eid.ui.fragments.registration.RegistrationViewModel
import com.digitall.eid.ui.fragments.registration.flow.RegistrationFlowViewModel
import com.digitall.eid.ui.fragments.scanner.flow.ScanCodeFlowViewModel
import com.digitall.eid.ui.fragments.scanner.scanner.ScanCodeViewModel
import com.digitall.eid.ui.fragments.start.flow.StartFlowViewModel
import com.digitall.eid.ui.fragments.start.splash.SplashViewModel
import com.digitall.eid.ui.fragments.termsandconditions.TermsAndConditionsViewModel
import com.digitall.eid.ui.fragments.termsandconditions.flow.TermsAndConditionsFlowViewModel
import org.koin.core.module.dsl.viewModel
import org.koin.dsl.module

val viewModelsModule = module {
    
    // Common

    viewModel<CommonBottomSheetWithSearchViewModel> {
        CommonBottomSheetWithSearchViewModel()
    }

    viewModel<CommonBottomSheetWithSearchMultiselectViewModel> {
        CommonBottomSheetWithSearchMultiselectViewModel()
    }

    viewModel<StartFlowViewModel> {
        StartFlowViewModel()
    }

    viewModel<SplashViewModel> {
        SplashViewModel()
    }

    viewModel<PermissionBottomSheetViewModel> {
        PermissionBottomSheetViewModel()
    }

    // Auth

    viewModel<AuthFlowViewModel> {
        AuthFlowViewModel()
    }

    viewModel<AuthStartViewModel> {
        AuthStartViewModel()
    }

    viewModel<AuthEnterBiometricViewModel> {
        AuthEnterBiometricViewModel()
    }

    viewModel<AuthEnterEmailPasswordViewModel> {
        AuthEnterEmailPasswordViewModel()
    }

    viewModel<AuthForgottenPasswordViewModel> {
        AuthForgottenPasswordViewModel()
    }

    // Registration

    viewModel<RegistrationFlowViewModel> {
        RegistrationFlowViewModel()
    }

    viewModel<RegistrationViewModel> {
        RegistrationViewModel()
    }

    // Main

    viewModel<MainViewModel> {
        MainViewModel()
    }

    viewModel<MainTabsFlowViewModel> {
        MainTabsFlowViewModel()
    }

    viewModel<BiometricErrorBottomSheetViewModel> {
        BiometricErrorBottomSheetViewModel()
    }

    viewModel<MainTabMoreViewModel> {
        MainTabMoreViewModel()
    }

    viewModel<MainTabHomeViewModel> {
        MainTabHomeViewModel()
    }

    viewModel<MainTabRequestsViewModel> {
        MainTabRequestsViewModel()
    }

    viewModel<MainTabEIMViewModel> {
        MainTabEIMViewModel()
    }


    // Notifications

    viewModel<NotificationsFlowViewModel> {
        NotificationsFlowViewModel()
    }

    viewModel<NotificationsPagerViewModel> {
        NotificationsPagerViewModel()
    }

    viewModel<NotificationsViewModel> {
        NotificationsViewModel()
    }

    viewModel<NotificationChannelsViewModel> {
        NotificationChannelsViewModel()
    }


    // Empowerment

    viewModel<EmpowermentFlowViewModel> {
        EmpowermentFlowViewModel()
    }

    viewModel<EmpowermentFromMeFlowViewModel> {
        EmpowermentFromMeFlowViewModel()
    }

    viewModel<EmpowermentToMeFlowViewModel> {
        EmpowermentToMeFlowViewModel()
    }

    viewModel<EmpowermentCreateFlowViewModel> {
        EmpowermentCreateFlowViewModel()
    }

    viewModel<EmpowermentIntroViewModel> {
        EmpowermentIntroViewModel()
    }

    viewModel<EmpowermentToMeViewModel> {
        EmpowermentToMeViewModel()
    }

    viewModel<EmpowermentFromMeViewModel> {
        EmpowermentFromMeViewModel()
    }

    viewModel<EmpowermentCreateViewModel> {
        EmpowermentCreateViewModel()
    }

    viewModel<EmpowermentFromMeFilterViewModel> {
        EmpowermentFromMeFilterViewModel()
    }

    viewModel<EmpowermentFromMeDetailsViewModel> {
        EmpowermentFromMeDetailsViewModel()
    }

    viewModel<EmpowermentCreatePreviewViewModel> {
        EmpowermentCreatePreviewViewModel()
    }

    viewModel<EmpowermentToMeDetailsViewModel> {
        EmpowermentToMeDetailsViewModel()
    }

    viewModel<EmpowermentToMeFilterViewModel> {
        EmpowermentToMeFilterViewModel()
    }

    viewModel<EmpowermentFromMeCancelViewModel> {
        EmpowermentFromMeCancelViewModel()
    }

    viewModel<EmpowermentToMeCancelViewModel> {
        EmpowermentToMeCancelViewModel()
    }

    viewModel<EmpowermentFromMeSigningViewModel> {
        EmpowermentFromMeSigningViewModel()
    }

    viewModel<EmpowermentLegalFlowViewModel> {
        EmpowermentLegalFlowViewModel()
    }

    viewModel<EmpowermentLegalSearchViewModel> {
        EmpowermentLegalSearchViewModel()
    }

    viewModel<EmpowermentLegalViewModel> {
        EmpowermentLegalViewModel()
    }

    viewModel<EmpowermentLegalFilterViewModel> {
        EmpowermentLegalFilterViewModel()
    }


    // Journal

    viewModel<JournalFlowViewModel> {
        JournalFlowViewModel()
    }

    viewModel<JournalIntroViewModel> {
        JournalIntroViewModel()
    }

    viewModel<JournalToMeViewModel> {
        JournalToMeViewModel()
    }

    viewModel<JournalFromMeViewModel> {
        JournalFromMeViewModel()
    }

    viewModel<JournalFromMeFlowViewModel> {
        JournalFromMeFlowViewModel()
    }

    viewModel<JournalFromMeFilterViewModel> {
        JournalFromMeFilterViewModel()
    }

    viewModel<JournalToMeFlowViewModel> {
        JournalToMeFlowViewModel()
    }

    viewModel<JournalToMeFilterViewModel> {
        JournalToMeFilterViewModel()
    }

    // certificates

    viewModel<CertificatesFlowViewModel> {
        CertificatesFlowViewModel()
    }

    viewModel<CertificatesViewModel> {
        CertificatesViewModel()
    }

    viewModel<CertificateDetailsViewModel> {
        CertificateDetailsViewModel()
    }

    viewModel<CertificateFilterViewModel> {
        CertificateFilterViewModel()
    }

    viewModel<CertificateStopViewModel> {
        CertificateStopViewModel()
    }

    viewModel<CertificateRevokeViewModel> {
        CertificateRevokeViewModel()
    }

    viewModel<CertificateResumeViewModel> {
        CertificateResumeViewModel()
    }

    viewModel<CertificateEditAliasViewModel> {
        CertificateEditAliasViewModel()
    }

    // applications

    viewModel<ApplicationsFlowViewModel> {
        ApplicationsFlowViewModel()
    }

    viewModel<ApplicationsViewModel> {
        ApplicationsViewModel()
    }

    viewModel<ApplicationDetailsViewModel> {
        ApplicationDetailsViewModel()
    }

    viewModel<ApplicationFilterViewModel> {
        ApplicationFilterViewModel()
    }

    viewModel<ApplicationCreateFlowViewModel> {
        ApplicationCreateFlowViewModel()
    }

    viewModel<ApplicationCreateIntroViewModel> {
        ApplicationCreateIntroViewModel()
    }

    viewModel<ApplicationCreatePreviewViewModel> {
        ApplicationCreatePreviewViewModel()
    }

    viewModel<ApplicationConfirmFlowViewModel> {
        ApplicationConfirmFlowViewModel()
    }

    viewModel<ApplicationConfirmIntroViewModel> {
        ApplicationConfirmIntroViewModel()
    }

    viewModel<ApplicationCreatePinViewModel> {
        ApplicationCreatePinViewModel()
    }

    viewModel<ApplicationContinueCreationFlowViewModel> {
        ApplicationContinueCreationFlowViewModel()
    }

    viewModel<ApplicationContinueCreationViewModel> {
        ApplicationContinueCreationViewModel()
    }

    viewModel<ApplicationContinueCreationCreatePinViewModel> {
        ApplicationContinueCreationCreatePinViewModel()
    }

    // scan code

    viewModel<ScanCodeFlowViewModel> {
        ScanCodeFlowViewModel()
    }

    viewModel<ScanCodeViewModel> {
        ScanCodeViewModel()
    }

    // change user email

    viewModel<ChangeCitizenEmailFlowViewModel> {
        ChangeCitizenEmailFlowViewModel()
    }

    viewModel<ChangeCitizenEmailViewModel> {
        ChangeCitizenEmailViewModel()
    }

    // change user password

    viewModel<ChangeCitizenPasswordFlowViewModel> {
        ChangeCitizenPasswordFlowViewModel()
    }

    viewModel<ChangeCitizenPasswordViewModel> {
        ChangeCitizenPasswordViewModel()
    }

    // change user password

    viewModel<ChangeCitizenInformationFlowViewModel> {
        ChangeCitizenInformationFlowViewModel()
    }

    viewModel<ChangeCitizenInformationViewModel> {
        ChangeCitizenInformationViewModel()
    }

    // user information

    viewModel<CitizenInformationFlowViewModel> {
        CitizenInformationFlowViewModel()
    }

    viewModel<CitizenInformationViewModel> {
        CitizenInformationViewModel()
    }

    // faq

    viewModel<FaqFlowViewModel> {
        FaqFlowViewModel()
    }

    viewModel<FaqViewModel> {
        FaqViewModel()
    }

    // contacts

    viewModel<ContactsFlowViewModel> {
        ContactsFlowViewModel()
    }

    viewModel<ContactsViewModel> {
        ContactsViewModel()
    }

    // terms and conditions

    viewModel<TermsAndConditionsFlowViewModel> {
        TermsAndConditionsFlowViewModel()
    }

    viewModel<TermsAndConditionsViewModel> {
        TermsAndConditionsViewModel()
    }

    // change card pin

    viewModel<CertificateChangePinFlowViewModel> {
        CertificateChangePinFlowViewModel()
    }

    viewModel<CertificateChangePinViewModel> {
        CertificateChangePinViewModel()
    }

    // Enter card pin

    viewModel<CardEnterPinFlowViewModel> {
        CardEnterPinFlowViewModel()
    }

    viewModel<CardEnterPinViewModel> {
        CardEnterPinViewModel()
    }

    // Scan card

    viewModel<ScanCardBottomSheetViewModel> {
        ScanCardBottomSheetViewModel()
    }

    // Auth card

    viewModel<AuthCardBottomSheetViewModel> {
        AuthCardBottomSheetViewModel()
    }

    // application create pin

    viewModel<ApplicationConfirmPinBottomSheetViewModel> {
        ApplicationConfirmPinBottomSheetViewModel()
    }

    viewModel<CertificateEnterPinViewModel> {
        CertificateEnterPinViewModel()
    }

    viewModel<CertificateEnterPinFlowViewModel> {
        CertificateEnterPinFlowViewModel()
    }

    // Administrators

    viewModel<AdministratorsViewModel> {
        AdministratorsViewModel()
    }

    viewModel<AdministratorsFlowViewModel> {
        AdministratorsFlowViewModel()
    }

    // Providers electronic administrative services

    viewModel<ProvidersElectronicAdministrativeServicesViewModel> {
        ProvidersElectronicAdministrativeServicesViewModel()
    }

    viewModel<ProvidersElectronicAdministrativeServicesFlowViewModel> {
        ProvidersElectronicAdministrativeServicesFlowViewModel()
    }

    // Electronic delivery system

    viewModel<ElectronicDeliverySystemViewModel> {
        ElectronicDeliverySystemViewModel()
    }

    viewModel<ElectronicDeliverySystemFlowViewModel> {
        ElectronicDeliverySystemFlowViewModel()
    }

    // Centers Certification service

    viewModel<CentersCertificationServicesViewModel> {
        CentersCertificationServicesViewModel()
    }

    viewModel<CentersCertificationServicesFlowViewModel> {
        CentersCertificationServicesFlowViewModel()
    }

    // Information bottom sheet

    viewModel<InformationBottomSheetViewModel> {
        InformationBottomSheetViewModel()
    }

    // Online help system

    viewModel<OnlineHelpSystemViewModel> {
        OnlineHelpSystemViewModel()
    }

    viewModel<OnlineHelpSystemFlowViewModel> {
        OnlineHelpSystemFlowViewModel()
    }

    // Application payment

    viewModel<ApplicationPaymentViewModel> {
        ApplicationPaymentViewModel()
    }

    // Create empowerment indefinite information

    viewModel<EmpowermentCreateIndefiniteBottomSheetViewModel> {
        EmpowermentCreateIndefiniteBottomSheetViewModel()
    }

    // Payments history

    viewModel<PaymentsHistoryFlowViewModel> {
        PaymentsHistoryFlowViewModel()
    }

    viewModel<PaymentsHistoryViewModel> {
        PaymentsHistoryViewModel()
    }

    viewModel<PaymentsHistoryFilterViewModel> {
        PaymentsHistoryFilterViewModel()
    }

    // MFA

    viewModel<AuthMfaViewModel> {
        AuthMfaViewModel()
    }

    // Bottom sheet empowerment withdrawal

    viewModel<EmpowermentFromMeCancelWithdrawalBottomSheetViewModel> {
        EmpowermentFromMeCancelWithdrawalBottomSheetViewModel()
    }

    // Bottom sheet create citizen profile pin

    viewModel<CreatePinCitizenProfileBottomSheetViewModel> {
        CreatePinCitizenProfileBottomSheetViewModel()
    }

    // Bottom sheet enter citizen profile pin

    viewModel<EnterPinCitizenProfileBottomSheetViewModel> {
        EnterPinCitizenProfileBottomSheetViewModel()
    }

    // Citizen profile security

    viewModel<CitizenProfileSecurityFlowViewModel> {
        CitizenProfileSecurityFlowViewModel()
    }

    viewModel<CitizenProfileSecurityViewModel> {
        CitizenProfileSecurityViewModel()
    }

}