/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.di

import com.digitall.eid.mappers.applications.create.intro.CreateApplicationIntroUiMapper
import com.digitall.eid.mappers.applications.show.details.ApplicationDetailsUiMapper
import com.digitall.eid.mappers.applications.show.all.ApplicationsUiMapper
import com.digitall.eid.mappers.auth.password.forgotten.AuthForgottenPasswordUiMapper
import com.digitall.eid.mappers.certificates.details.CertificateDetailsUiMapper
import com.digitall.eid.mappers.certificates.all.CertificatesUiMapper
import com.digitall.eid.mappers.certificates.change.pin.CertificateChangePinUiMapper
import com.digitall.eid.mappers.certificates.edit.alias.CertificateEditAliasUiMapper
import com.digitall.eid.mappers.certificates.resume.CertificateResumeUiMapper
import com.digitall.eid.mappers.certificates.revoke.CertificateRevokeUiMapper
import com.digitall.eid.mappers.certificates.stop.CertificateStopUiMapper
import com.digitall.eid.mappers.citizen.change.email.ChangeCitizenEmailUiMapper
import com.digitall.eid.mappers.citizen.change.information.ChangeCitizenInformationUiMapper
import com.digitall.eid.mappers.citizen.change.password.ChangeCitizenPasswordUiMapper
import com.digitall.eid.mappers.citizen.profile.security.CitizenProfileSecurityUiMapper
import com.digitall.eid.mappers.common.CreateCodeResponseErrorToStringMapper
import com.digitall.eid.mappers.common.PermissionNamePmMapper
import com.digitall.eid.mappers.empowerment.common.all.EmpowermentSortingModelUiMapper
import com.digitall.eid.mappers.empowerment.create.CreateEmpowermentFromCompanyUiMapper
import com.digitall.eid.mappers.empowerment.create.CreateEmpowermentFromPersonUiMapper
import com.digitall.eid.mappers.empowerment.from.me.all.EmpowermentFromMeUiMapper
import com.digitall.eid.mappers.empowerment.from.me.details.EmpowermentFromMeDetailsUiMapper
import com.digitall.eid.mappers.empowerment.legal.all.EmpowermentLegalUiMapper
import com.digitall.eid.mappers.empowerment.to.me.all.EmpowermentToMeUiMapper
import com.digitall.eid.mappers.empowerment.to.me.details.EmpowermentToMeDetailsUiMapper
import com.digitall.eid.mappers.journal.JournalFromMeUiMapper
import com.digitall.eid.mappers.journal.JournalToMeUiMapper
import com.digitall.eid.mappers.main.tabs.more.MainTabMoreUiMapper
import com.digitall.eid.mappers.notifications.channels.NotificationChannelUiMapper
import com.digitall.eid.mappers.notifications.notifications.NotificationUiMapper
import com.digitall.eid.mappers.payments.history.PaymentsHistoryUiMapper
import com.digitall.eid.mappers.registration.RegistrationUiMapper
import com.digitall.eid.mappers.requests.RequestUiMapper
import org.koin.dsl.module

val mappersModule = module {

    single<PermissionNamePmMapper> {
        PermissionNamePmMapper()
    }

    single<CreateCodeResponseErrorToStringMapper> {
        CreateCodeResponseErrorToStringMapper()
    }

    single<NotificationChannelUiMapper> {
        NotificationChannelUiMapper()
    }

    single<NotificationUiMapper> {
        NotificationUiMapper()
    }

    single<MainTabMoreUiMapper> {
        MainTabMoreUiMapper()
    }

    single<EmpowermentFromMeUiMapper> {
        EmpowermentFromMeUiMapper()
    }

    single<EmpowermentSortingModelUiMapper> {
        EmpowermentSortingModelUiMapper()
    }

    single<EmpowermentFromMeDetailsUiMapper> {
        EmpowermentFromMeDetailsUiMapper()
    }

    single<EmpowermentToMeUiMapper> {
        EmpowermentToMeUiMapper()
    }

    single<EmpowermentToMeDetailsUiMapper> {
        EmpowermentToMeDetailsUiMapper()
    }

    single<EmpowermentLegalUiMapper> {
        EmpowermentLegalUiMapper()
    }

    single<JournalFromMeUiMapper> {
        JournalFromMeUiMapper()
    }

    single<JournalToMeUiMapper> {
        JournalToMeUiMapper()
    }

    single<ApplicationsUiMapper> {
        ApplicationsUiMapper()
    }

    single<CertificatesUiMapper> {
        CertificatesUiMapper()
    }

    single<CertificateDetailsUiMapper> {
        CertificateDetailsUiMapper()
    }

    single<ApplicationDetailsUiMapper> {
        ApplicationDetailsUiMapper()
    }

    single<RequestUiMapper> {
        RequestUiMapper()
    }

    single<PaymentsHistoryUiMapper> {
        PaymentsHistoryUiMapper()
    }

    single<AuthForgottenPasswordUiMapper> {
        AuthForgottenPasswordUiMapper()
    }

    single<RegistrationUiMapper> {
        RegistrationUiMapper()
    }

    single<ChangeCitizenInformationUiMapper> {
        ChangeCitizenInformationUiMapper()
    }

    single<ChangeCitizenEmailUiMapper> {
        ChangeCitizenEmailUiMapper()
    }

    single<ChangeCitizenPasswordUiMapper> {
        ChangeCitizenPasswordUiMapper()
    }

    single<CreateApplicationIntroUiMapper> {
        CreateApplicationIntroUiMapper()
    }

    single<CertificateChangePinUiMapper> {
        CertificateChangePinUiMapper()
    }

    single<CertificateEditAliasUiMapper> {
        CertificateEditAliasUiMapper()
    }

    single<CertificateResumeUiMapper> {
        CertificateResumeUiMapper()
    }

    single<CertificateRevokeUiMapper> {
        CertificateRevokeUiMapper()
    }

    single<CertificateStopUiMapper> {
        CertificateStopUiMapper()
    }

    single<CreateEmpowermentFromPersonUiMapper> {
        CreateEmpowermentFromPersonUiMapper()
    }

    single<CreateEmpowermentFromCompanyUiMapper> {
        CreateEmpowermentFromCompanyUiMapper()
    }

    single<CitizenProfileSecurityUiMapper> {
        CitizenProfileSecurityUiMapper()
    }
}