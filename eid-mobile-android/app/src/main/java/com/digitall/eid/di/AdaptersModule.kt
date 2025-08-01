/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.di

import com.digitall.eid.ui.fragments.applications.create.intro.adapter.ApplicationCreateIntroAdapter
import com.digitall.eid.ui.fragments.applications.create.preview.adapter.ApplicationCreatePreviewAdapter
import com.digitall.eid.ui.fragments.applications.show.all.list.ApplicationsAdapter
import com.digitall.eid.ui.fragments.applications.show.details.list.ApplicationDetailsAdapter
import com.digitall.eid.ui.fragments.applications.show.filter.list.ApplicationFilterAdapter
import com.digitall.eid.ui.fragments.auth.password.forgotten.list.AuthForgottenPasswordAdapter
import com.digitall.eid.ui.fragments.certificates.change.pin.list.CertificateChangePinAdapter
import com.digitall.eid.ui.fragments.certificates.all.list.CertificatesAdapter
import com.digitall.eid.ui.fragments.certificates.details.list.CertificateDetailsAdapter
import com.digitall.eid.ui.fragments.certificates.edit.alias.list.CertificateEditAliasAdapter
import com.digitall.eid.ui.fragments.certificates.filter.list.CertificateFilterAdapter
import com.digitall.eid.ui.fragments.certificates.resume.list.CertificateResumeAdapter
import com.digitall.eid.ui.fragments.certificates.revoke.list.CertificateRevokeAdapter
import com.digitall.eid.ui.fragments.certificates.stop.list.CertificateStopAdapter
import com.digitall.eid.ui.fragments.common.search.normal.list.CommonBottomSheetWithSearchAdapter
import com.digitall.eid.ui.fragments.empowerment.create.create.list.EmpowermentCreateAdapter
import com.digitall.eid.ui.fragments.empowerment.create.preview.list.EmpowermentCreatePreviewAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.all.list.EmpowermentFromMeAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.cancel.list.EmpowermentFromMeCancelAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.details.list.EmpowermentFromMeDetailsAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.filter.list.EmpowermentFromMeFilterAdapter
import com.digitall.eid.ui.fragments.empowerment.from.me.signing.list.EmpowermentFromMeSigningAdapter
import com.digitall.eid.ui.fragments.empowerment.legal.all.list.EmpowermentLegalAdapter
import com.digitall.eid.ui.fragments.empowerment.legal.filter.list.EmpowermentLegalFilterAdapter
import com.digitall.eid.ui.fragments.empowerment.legal.search.list.EmpowermentLegalSearchAdapter
import com.digitall.eid.ui.fragments.empowerment.to.me.all.list.EmpowermentToMeAdapter
import com.digitall.eid.ui.fragments.empowerment.to.me.cancel.list.EmpowermentToMeCancelAdapter
import com.digitall.eid.ui.fragments.empowerment.to.me.details.list.EmpowermentToMeDetailsAdapter
import com.digitall.eid.ui.fragments.empowerment.to.me.filter.list.EmpowermentToMeFilterAdapter
import com.digitall.eid.ui.fragments.journal.from.me.all.list.JournalFromMeAdapter
import com.digitall.eid.ui.fragments.journal.to.me.all.list.JournalToMeAdapter
import com.digitall.eid.ui.fragments.main.tabs.more.list.TabMoreAdapter
import com.digitall.eid.ui.fragments.notifications.channels.list.NotificationChannelsAdapter
import com.digitall.eid.ui.fragments.notifications.notifications.list.NotificationsAdapter
import com.digitall.eid.ui.fragments.registration.list.RegistrationAdapter
import com.digitall.eid.ui.fragments.citizen.change.email.list.ChangeCitizenEmailAdapter
import com.digitall.eid.ui.fragments.citizen.change.password.list.ChangeCitizenPasswordAdapter
import com.digitall.eid.ui.fragments.citizen.change.information.list.ChangeCitizenInformationAdapter
import com.digitall.eid.ui.fragments.citizen.information.list.CitizenInformationAdapter
import com.digitall.eid.ui.fragments.citizen.profile.security.list.CitizenProfileSecurityAdapter
import com.digitall.eid.ui.fragments.common.search.multiselect.list.CommonBottomSheetWithSearchMultiselectAdapter
import com.digitall.eid.ui.fragments.journal.from.me.filter.list.JournalFromMeFilterAdapter
import com.digitall.eid.ui.fragments.journal.to.me.filter.list.JournalToMeFilterAdapter
import com.digitall.eid.ui.fragments.main.tabs.requests.list.TabRequestsAdapter
import com.digitall.eid.ui.fragments.payments.filter.list.PaymentsHistoryFilterAdapter
import com.digitall.eid.ui.fragments.payments.history.list.PaymentsHistoryAdapter
import org.koin.dsl.module

val adaptersModule = module {

    single<TabMoreAdapter> {
        TabMoreAdapter()
    }

    single<NotificationChannelsAdapter> {
        NotificationChannelsAdapter()
    }

    single<NotificationsAdapter> {
        NotificationsAdapter()
    }

    single<EmpowermentCreateAdapter> {
        EmpowermentCreateAdapter()
    }

    single<CommonBottomSheetWithSearchAdapter> {
        CommonBottomSheetWithSearchAdapter()
    }

    single<CommonBottomSheetWithSearchMultiselectAdapter> {
        CommonBottomSheetWithSearchMultiselectAdapter()
    }

    single<EmpowermentFromMeAdapter> {
        EmpowermentFromMeAdapter()
    }

    single<EmpowermentFromMeFilterAdapter> {
        EmpowermentFromMeFilterAdapter()
    }

    single<EmpowermentToMeFilterAdapter> {
        EmpowermentToMeFilterAdapter()
    }

    single<EmpowermentToMeAdapter> {
        EmpowermentToMeAdapter()
    }

    single<EmpowermentFromMeDetailsAdapter> {
        EmpowermentFromMeDetailsAdapter()
    }

    single<EmpowermentLegalAdapter> {
        EmpowermentLegalAdapter()
    }

    single<EmpowermentLegalFilterAdapter> {
        EmpowermentLegalFilterAdapter()
    }

    single<ApplicationDetailsAdapter> {
        ApplicationDetailsAdapter()
    }

    single<ApplicationCreateIntroAdapter> {
        ApplicationCreateIntroAdapter()
    }

    single<ApplicationCreatePreviewAdapter> {
        ApplicationCreatePreviewAdapter()
    }

    single<CertificateDetailsAdapter> {
        CertificateDetailsAdapter()
    }

    single<EmpowermentCreatePreviewAdapter> {
        EmpowermentCreatePreviewAdapter()
    }

    single<EmpowermentToMeDetailsAdapter> {
        EmpowermentToMeDetailsAdapter()
    }

    single<EmpowermentFromMeCancelAdapter> {
        EmpowermentFromMeCancelAdapter()
    }

    single<EmpowermentToMeCancelAdapter> {
        EmpowermentToMeCancelAdapter()
    }

    single<EmpowermentFromMeSigningAdapter> {
        EmpowermentFromMeSigningAdapter()
    }

    single<JournalFromMeAdapter> {
        JournalFromMeAdapter()
    }

    single<JournalToMeAdapter> {
        JournalToMeAdapter()
    }

    single<JournalFromMeFilterAdapter> {
        JournalFromMeFilterAdapter()
    }

    single<JournalToMeFilterAdapter> {
        JournalToMeFilterAdapter()
    }

    single<ApplicationsAdapter> {
        ApplicationsAdapter()
    }

    single<CertificatesAdapter> {
        CertificatesAdapter()
    }

    single<ApplicationFilterAdapter> {
        ApplicationFilterAdapter()
    }

    single<CertificateFilterAdapter> {
        CertificateFilterAdapter()
    }

    single<CertificateStopAdapter> {
        CertificateStopAdapter()
    }

    single<CertificateResumeAdapter> {
        CertificateResumeAdapter()
    }

    single<CertificateRevokeAdapter> {
        CertificateRevokeAdapter()
    }

    single<RegistrationAdapter> {
        RegistrationAdapter()
    }

    single<AuthForgottenPasswordAdapter> {
        AuthForgottenPasswordAdapter()
    }

    single<CertificateEditAliasAdapter> {
        CertificateEditAliasAdapter()
    }

    // change user email

    single<ChangeCitizenEmailAdapter> {
        ChangeCitizenEmailAdapter()
    }

    // change user email

    single<ChangeCitizenEmailAdapter> {
        ChangeCitizenEmailAdapter()
    }

    // change user password

    single<ChangeCitizenPasswordAdapter> {
        ChangeCitizenPasswordAdapter()
    }

    // change user phone

    single<ChangeCitizenInformationAdapter> {
        ChangeCitizenInformationAdapter()
    }

    // User information

    single<CitizenInformationAdapter> {
        CitizenInformationAdapter()
    }

    single<EmpowermentLegalSearchAdapter> {
        EmpowermentLegalSearchAdapter()
    }

    single<CertificateChangePinAdapter> {
        CertificateChangePinAdapter()
    }

    // requests

    single<TabRequestsAdapter> {
        TabRequestsAdapter()
    }

    // Payments history

    single<PaymentsHistoryAdapter> {
        PaymentsHistoryAdapter()
    }

    single<PaymentsHistoryFilterAdapter> {
        PaymentsHistoryFilterAdapter()
    }

    single<CitizenProfileSecurityAdapter> {
        CitizenProfileSecurityAdapter()
    }

}