/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.network.administrators.AdministratorsApi
import com.digitall.eid.data.network.applications.ApplicationCreateApi
import com.digitall.eid.data.network.applications.ApplicationsApi
import com.digitall.eid.data.network.assets.AssetsApi
import com.digitall.eid.data.network.authentication.AuthenticationApi
import com.digitall.eid.data.network.certificates.CertificatesApi
import com.digitall.eid.data.network.citizen.eid.associate.CitizenEidAssociateApi
import com.digitall.eid.data.network.citizen.forgotten.CitizenForgottenApi
import com.digitall.eid.data.network.citizen.registration.CitizenRegistrationApi
import com.digitall.eid.data.network.citizen.update.CitizenUpdateApi
import com.digitall.eid.data.network.devices.DevicesApi
import com.digitall.eid.data.network.empowerment.EmpowermentApi
import com.digitall.eid.data.network.empowerment.EmpowermentCancelApi
import com.digitall.eid.data.network.empowerment.EmpowermentCreateApi
import com.digitall.eid.data.network.events.EventsApi
import com.digitall.eid.data.network.journal.JournalApi
import com.digitall.eid.data.network.mfa.MfaApi
import com.digitall.eid.data.network.nomenclatures.NomenclaturesApi
import com.digitall.eid.data.network.notifications.NotificationsApi
import com.digitall.eid.data.network.payments.PaymentsApi
import com.digitall.eid.data.network.requests.RequestsApi
import com.digitall.eid.data.network.signing.SigningApi
import com.digitall.eid.data.network.verify.login.VerifyLoginApi
import org.koin.core.qualifier.named
import org.koin.dsl.module
import retrofit2.Retrofit

val apiModule = module {

    single {
        get<Retrofit>(named(RETROFIT_ISCEIGW)).create(AuthenticationApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_ISCEIGW)).create(RequestsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_PAN)).create(NotificationsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_RO)).create(EmpowermentCreateApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_RO)).create(EmpowermentApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_RO)).create(EmpowermentCancelApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_SIGNING)).create(SigningApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_PJS)).create(JournalApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(ApplicationsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(ApplicationCreateApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(CertificatesApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(NomenclaturesApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(CitizenRegistrationApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(CitizenUpdateApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(CitizenForgottenApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(VerifyLoginApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_RAEICEI)).create(DevicesApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_RAEICEI)).create(AdministratorsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_BASE)).create(AssetsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(EventsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_MPOZEI)).create(PaymentsApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_ISCEIGW)).create(CitizenEidAssociateApi::class.java)
    }

    single {
        get<Retrofit>(named(RETROFIT_ISCEIGW)).create(MfaApi::class.java)
    }

}