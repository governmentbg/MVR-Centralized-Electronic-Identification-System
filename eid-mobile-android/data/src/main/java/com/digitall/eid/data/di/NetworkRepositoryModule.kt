/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.repository.network.administrators.AdministratorsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.applications.ApplicationCreateNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.applications.ApplicationsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.assets.AssetsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.authentication.AuthenticationNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.certificates.CertificatesNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.citizen.eid.associate.CitizenEidAssociateNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.citizen.forgotten.CitizenForgottenNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.citizen.registration.CitizenRegistrationNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.citizen.update.CitizenUpdateNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.devices.DevicesNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.empowerment.EmpowermentCancelNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.empowerment.EmpowermentCreateNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.empowerment.EmpowermentNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.empowerment.EmpowermentSigningNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.events.EventsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.journal.JournalNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.mfa.MfaNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.nomenclatures.NomenclaturesNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.notifications.channels.NotificationChannelsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.notifications.notifications.NotificationsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.payments.history.PaymentsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.requests.RequestsNetworkRepositoryImpl
import com.digitall.eid.data.repository.network.signing.SigningNetworkRepositoryImpl
import com.digitall.eid.domain.repository.network.administrators.AdministratorsNetworkRepository
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.repository.network.applications.ApplicationsNetworkRepository
import com.digitall.eid.domain.repository.network.assets.AssetsNetworkRepository
import com.digitall.eid.domain.repository.network.authentication.AuthenticationNetworkRepository
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.repository.network.citizen.eid.associate.CitizenEidAssociateNetworkRepository
import com.digitall.eid.domain.repository.network.citizen.forgotten.CitizenForgottenNetworkRepository
import com.digitall.eid.domain.repository.network.citizen.registration.CitizenRegistrationNetworkRepository
import com.digitall.eid.domain.repository.network.citizen.update.CitizenUpdateNetworkRepository
import com.digitall.eid.domain.repository.network.devices.DevicesNetworkRepository
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCancelNetworkRepository
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCreateNetworkRepository
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentNetworkRepository
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentSigningNetworkRepository
import com.digitall.eid.domain.repository.network.events.EventsNetworkRepository
import com.digitall.eid.domain.repository.network.journal.JournalNetworkRepository
import com.digitall.eid.domain.repository.network.mfa.MfaNetworkRepository
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.repository.network.notifications.channels.NotificationChannelsNetworkRepository
import com.digitall.eid.domain.repository.network.notifications.notifications.NotificationsNetworkRepository
import com.digitall.eid.domain.repository.network.payments.PaymentsNetworkRepository
import com.digitall.eid.domain.repository.network.requests.RequestsNetworkRepository
import com.digitall.eid.domain.repository.network.signing.SigningNetworkRepository
import org.koin.dsl.module

val networkRepositoryModule = module {

    single<AuthenticationNetworkRepository> {
        AuthenticationNetworkRepositoryImpl()
    }

    single<NotificationsNetworkRepository> {
        NotificationsNetworkRepositoryImpl()
    }

    single<NotificationChannelsNetworkRepository> {
        NotificationChannelsNetworkRepositoryImpl()
    }

    single<EmpowermentNetworkRepository> {
        EmpowermentNetworkRepositoryImpl()
    }

    single<EmpowermentCancelNetworkRepository> {
        EmpowermentCancelNetworkRepositoryImpl()
    }

    single<EmpowermentCreateNetworkRepository> {
        EmpowermentCreateNetworkRepositoryImpl()
    }

    single<EmpowermentSigningNetworkRepository> {
        EmpowermentSigningNetworkRepositoryImpl()
    }

    single<JournalNetworkRepository> {
        JournalNetworkRepositoryImpl()
    }

    // applications

    single<ApplicationsNetworkRepository> {
        ApplicationsNetworkRepositoryImpl()
    }

    single<ApplicationCreateNetworkRepository> {
        ApplicationCreateNetworkRepositoryImpl()
    }

    // certificates

    single<CertificatesNetworkRepository> {
        CertificatesNetworkRepositoryImpl()
    }

    // registration

    single<CitizenRegistrationNetworkRepositoryImpl> {
        CitizenRegistrationNetworkRepositoryImpl()
    }

    // update

    single<CitizenUpdateNetworkRepository> {
        CitizenUpdateNetworkRepositoryImpl()
    }

    // nomenclatures

    single<NomenclaturesNetworkRepository> {
        NomenclaturesNetworkRepositoryImpl()
    }

    // registration

    single<CitizenRegistrationNetworkRepository> {
        CitizenRegistrationNetworkRepositoryImpl()
    }

    // update

    single<CitizenUpdateNetworkRepository> {
        CitizenUpdateNetworkRepositoryImpl()
    }

    // forgotten

    single<CitizenForgottenNetworkRepository> {
        CitizenForgottenNetworkRepositoryImpl()
    }

    // signing

    single<SigningNetworkRepository> {
        SigningNetworkRepositoryImpl()
    }

    // devices

    single<DevicesNetworkRepository> {
        DevicesNetworkRepositoryImpl()
    }

    // administrators

    single<AdministratorsNetworkRepository> {
        AdministratorsNetworkRepositoryImpl()
    }

    // requests

    single<RequestsNetworkRepository> {
        RequestsNetworkRepositoryImpl()
    }

    // assets

    single<AssetsNetworkRepository> {
        AssetsNetworkRepositoryImpl()
    }

    // events
    single<EventsNetworkRepository> {
        EventsNetworkRepositoryImpl()
    }

    // payments

    single<PaymentsNetworkRepository> {
        PaymentsNetworkRepositoryImpl()
    }

    // associate eid

    single<CitizenEidAssociateNetworkRepository> {
        CitizenEidAssociateNetworkRepositoryImpl()
    }

    // MFA

    single<MfaNetworkRepository> {
        MfaNetworkRepositoryImpl()
    }

}