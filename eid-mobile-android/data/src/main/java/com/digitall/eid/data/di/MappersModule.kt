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
package com.digitall.eid.data.di

import com.digitall.eid.data.mappers.database.notifications.channels.NotificationChannelSelectedEntityMapper
import com.digitall.eid.data.mappers.database.notifications.notifications.NotificationEntityMapper
import com.digitall.eid.data.mappers.database.notifications.notifications.NotificationsNotSelectedEntityMapper
import com.digitall.eid.data.mappers.network.administrators.AdministratorFrontOfficesResponseMapper
import com.digitall.eid.data.mappers.network.administrators.AdministratorsResponseMapper
import com.digitall.eid.data.mappers.network.applications.all.ApplicationCompletionStatusResponseMapper
import com.digitall.eid.data.mappers.network.applications.all.ApplicationDetailsResponseMapper
import com.digitall.eid.data.mappers.network.applications.all.ApplicationsResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationConfirmWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationConfirmWithEIDRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithBaseProfileResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithEIDRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationEnrollWithEIDResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationGenerateUserDetailsXMLResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSendSignatureRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSendSignatureResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationSignWithBaseProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUpdateProfileRequestMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUpdateProfileResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUserDetailsResponseMapper
import com.digitall.eid.data.mappers.network.applications.create.ApplicationUserDetailsXMLRequestMapper
import com.digitall.eid.data.mappers.network.assets.AssetsLocalizationsResponseMapper
import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationChallengeRequestMapper
import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationWithBasicProfileRequestMapper
import com.digitall.eid.data.mappers.network.authentication.request.AuthenticationWithCertificateRequestMapper
import com.digitall.eid.data.mappers.network.authentication.response.AuthenticationChallengeResponseMapper
import com.digitall.eid.data.mappers.network.authentication.response.AuthenticationResponseMapper
import com.digitall.eid.data.mappers.network.certificates.request.CertificateAliasChangeRequestMapper
import com.digitall.eid.data.mappers.network.certificates.request.CertificateChangeStatusRequestMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateChangeStatusResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateDetailsResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificateHistoryResponseMapper
import com.digitall.eid.data.mappers.network.certificates.response.CertificatesResponseMapper
import com.digitall.eid.data.mappers.network.citizen.eid.associate.request.CitizenEidAssociateRequestMapper
import com.digitall.eid.data.mappers.network.citizen.forgotten.password.request.CitizenForgottenPasswordRequestMapper
import com.digitall.eid.data.mappers.network.citizen.registration.request.RegisterNewCitizenRequestMapper
import com.digitall.eid.data.mappers.network.citizen.update.email.request.CitizenUpdateEmailRequestMapper
import com.digitall.eid.data.mappers.network.citizen.update.infromation.request.CitizenUpdateInformationRequestMapper
import com.digitall.eid.data.mappers.network.devices.response.DevicesResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.common.all.EmpowermentRequestMapper
import com.digitall.eid.data.mappers.network.empowerment.common.all.EmpowermentResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentCreateRequestMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentProviderResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentServiceScopeGetResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentServicesResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.legal.EmpowermentLegalRequestMapper
import com.digitall.eid.data.mappers.network.empowerment.signing.EmpowermentReasonResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.signing.EmpowermentSigningSignRequestMapper
import com.digitall.eid.data.mappers.network.events.request.EventsRequestMapper
import com.digitall.eid.data.mappers.network.journal.JournalRequestMapper
import com.digitall.eid.data.mappers.network.journal.JournalResponseMapper
import com.digitall.eid.data.mappers.network.mfa.request.MfaGenerateNewOtpCodeRequestMapper
import com.digitall.eid.data.mappers.network.mfa.request.MfaVerifyOtpCodeRequestMapper
import com.digitall.eid.data.mappers.network.nomenclatures.reasons.NomenclaturesGetReasonsResponseMapper
import com.digitall.eid.data.mappers.network.notifications.channels.NotificationChannelsResponseMapper
import com.digitall.eid.data.mappers.network.notifications.channels.NotificationChannelsSelectedResponseMapper
import com.digitall.eid.data.mappers.network.notifications.notifications.NotificationsNotSelectedResponseMapper
import com.digitall.eid.data.mappers.network.notifications.notifications.NotificationsResponseMapper
import com.digitall.eid.data.mappers.network.payments.history.response.PaymentsHistoryResponseMapper
import com.digitall.eid.data.mappers.network.requests.request.RequestOutcomeRequestMapper
import com.digitall.eid.data.mappers.network.requests.response.RequestsResponseMapper
import com.digitall.eid.data.mappers.network.signing.SigningCheckUserStatusRequestMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaDownloadResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaSignRequestMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaSignResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.borica.SigningBoricaUserStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustDownloadResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustSignRequestMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustSignResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustStatusResponseMapper
import com.digitall.eid.data.mappers.network.signing.evrotrust.SigningEvrotrustUserStatusResponseMapper
import com.digitall.eid.data.mappers.network.user.UserMapper
import com.digitall.eid.data.mappers.network.verify.login.request.VerifyLoginRequestMapper
import com.digitall.eid.data.mappers.network.verify.login.response.VerifyLoginResponseMapper
import com.digitall.eid.data.utils.JWTDecoderImpl
import com.digitall.eid.domain.utils.JWTDecoder
import org.koin.dsl.module
import org.simpleframework.xml.core.Persister

val mappersModule = module {

    single<Persister> {
        Persister()
    }

    single<JWTDecoder> {
        JWTDecoderImpl()
    }

    single<NotificationChannelsResponseMapper> {
        NotificationChannelsResponseMapper()
    }

    single<NotificationsResponseMapper> {
        NotificationsResponseMapper()
    }

    single<NotificationChannelsSelectedResponseMapper> {
        NotificationChannelsSelectedResponseMapper()
    }

    single<NotificationsNotSelectedResponseMapper> {
        NotificationsNotSelectedResponseMapper()
    }

    single<NotificationEntityMapper> {
        NotificationEntityMapper()
    }

    single<NotificationsNotSelectedEntityMapper> {
        NotificationsNotSelectedEntityMapper()
    }

    single<NotificationChannelSelectedEntityMapper> {
        NotificationChannelSelectedEntityMapper()
    }

    single<AuthenticationResponseMapper> {
        AuthenticationResponseMapper()
    }

    single<AuthenticationWithBasicProfileRequestMapper> {
        AuthenticationWithBasicProfileRequestMapper()
    }

    single<AuthenticationWithCertificateRequestMapper> {
        AuthenticationWithCertificateRequestMapper()
    }

    single<AuthenticationChallengeRequestMapper> {
        AuthenticationChallengeRequestMapper()
    }

    single<AuthenticationChallengeResponseMapper> {
        AuthenticationChallengeResponseMapper()
    }

    single<EmpowermentRequestMapper> {
        EmpowermentRequestMapper()
    }

    single<EmpowermentResponseMapper> {
        EmpowermentResponseMapper()
    }

    single<EmpowermentServicesResponseMapper> {
        EmpowermentServicesResponseMapper()
    }

    single<EmpowermentProviderResponseMapper> {
        EmpowermentProviderResponseMapper()
    }

    single<EmpowermentCreateRequestMapper> {
        EmpowermentCreateRequestMapper()
    }

    single<EmpowermentServiceScopeGetResponseMapper> {
        EmpowermentServiceScopeGetResponseMapper()
    }

    single<EmpowermentReasonResponseMapper> {
        EmpowermentReasonResponseMapper()
    }

    single<EmpowermentLegalRequestMapper> {
        EmpowermentLegalRequestMapper()
    }

    single<SigningBoricaDownloadResponseMapper> {
        SigningBoricaDownloadResponseMapper()
    }

    single<SigningEvrotrustDownloadResponseMapper> {
        SigningEvrotrustDownloadResponseMapper()
    }

    single<SigningBoricaSignRequestMapper> {
        SigningBoricaSignRequestMapper()
    }

    single<SigningBoricaSignResponseMapper> {
        SigningBoricaSignResponseMapper()
    }

    single<SigningEvrotrustSignRequestMapper> {
        SigningEvrotrustSignRequestMapper()
    }

    single<SigningEvrotrustSignResponseMapper> {
        SigningEvrotrustSignResponseMapper()
    }

    single<EmpowermentSigningSignRequestMapper> {
        EmpowermentSigningSignRequestMapper()
    }

    single<SigningBoricaStatusResponseMapper> {
        SigningBoricaStatusResponseMapper()
    }

    single<SigningEvrotrustStatusResponseMapper> {
        SigningEvrotrustStatusResponseMapper()
    }

    single<SigningBoricaUserStatusResponseMapper> {
        SigningBoricaUserStatusResponseMapper()
    }

    single<SigningEvrotrustUserStatusResponseMapper> {
        SigningEvrotrustUserStatusResponseMapper()
    }

    single<JournalResponseMapper> {
        JournalResponseMapper()
    }

    single<JournalRequestMapper> {
        JournalRequestMapper()
    }

    single<NomenclaturesGetReasonsResponseMapper> {
        NomenclaturesGetReasonsResponseMapper()
    }

    single<ApplicationsResponseMapper> {
        ApplicationsResponseMapper()
    }

    single<CertificatesResponseMapper> {
        CertificatesResponseMapper()
    }

    single<CertificateDetailsResponseMapper> {
        CertificateDetailsResponseMapper()
    }

    single<CertificateHistoryResponseMapper> {
        CertificateHistoryResponseMapper()
    }

    single<ApplicationDetailsResponseMapper> {
        ApplicationDetailsResponseMapper()
    }


    single<ApplicationUserDetailsResponseMapper> {
        ApplicationUserDetailsResponseMapper()
    }

    single<ApplicationUserDetailsXMLRequestMapper> {
        ApplicationUserDetailsXMLRequestMapper()
    }

    single<ApplicationSendSignatureRequestMapper> {
        ApplicationSendSignatureRequestMapper()
    }

    single<ApplicationGenerateUserDetailsXMLResponseMapper> {
        ApplicationGenerateUserDetailsXMLResponseMapper()
    }

    single<ApplicationSendSignatureResponseMapper> {
        ApplicationSendSignatureResponseMapper()
    }

    single<RegisterNewCitizenRequestMapper> {
        RegisterNewCitizenRequestMapper()
    }

    single<CitizenUpdateEmailRequestMapper> {
        CitizenUpdateEmailRequestMapper()
    }

    single<RegisterNewCitizenRequestMapper> {
        RegisterNewCitizenRequestMapper()
    }

    single<CitizenUpdateEmailRequestMapper> {
        CitizenUpdateEmailRequestMapper()
    }

    single<CitizenUpdateInformationRequestMapper> {
        CitizenUpdateInformationRequestMapper()
    }

    single<CitizenForgottenPasswordRequestMapper> {
        CitizenForgottenPasswordRequestMapper()
    }

    single<ApplicationSignWithBaseProfileRequestMapper> {
        ApplicationSignWithBaseProfileRequestMapper()
    }

    single<ApplicationEnrollWithBaseProfileRequestMapper> {
        ApplicationEnrollWithBaseProfileRequestMapper()
    }

    single<ApplicationEnrollWithBaseProfileResponseMapper> {
        ApplicationEnrollWithBaseProfileResponseMapper()
    }

    single<ApplicationConfirmWithBaseProfileRequestMapper> {
        ApplicationConfirmWithBaseProfileRequestMapper()
    }

    single<ApplicationConfirmWithEIDRequestMapper> {
        ApplicationConfirmWithEIDRequestMapper()
    }

    single<ApplicationEnrollWithEIDRequestMapper> {
        ApplicationEnrollWithEIDRequestMapper()
    }

    single<ApplicationEnrollWithEIDResponseMapper> {
        ApplicationEnrollWithEIDResponseMapper()
    }

    single<ApplicationUpdateProfileRequestMapper> {
        ApplicationUpdateProfileRequestMapper()
    }

    single<ApplicationUpdateProfileResponseMapper> {
        ApplicationUpdateProfileResponseMapper()
    }

    single<CertificateChangeStatusRequestMapper> {
        CertificateChangeStatusRequestMapper()
    }

    single<CertificateChangeStatusResponseMapper> {
        CertificateChangeStatusResponseMapper()
    }

    single<DevicesResponseMapper> {
        DevicesResponseMapper()
    }

    single<UserMapper> {
        UserMapper()
    }

    single<VerifyLoginRequestMapper> {
        VerifyLoginRequestMapper()
    }

    single<VerifyLoginResponseMapper> {
        VerifyLoginResponseMapper()
    }

    // administrators

    single<AdministratorsResponseMapper> {
        AdministratorsResponseMapper()
    }

    single<AdministratorFrontOfficesResponseMapper> {
        AdministratorFrontOfficesResponseMapper()
    }

    single<VerifyLoginRequestMapper> {
        VerifyLoginRequestMapper()
    }

    single<VerifyLoginResponseMapper> {
        VerifyLoginResponseMapper()
    }

    single<RequestsResponseMapper> {
        RequestsResponseMapper()
    }

    single<RequestOutcomeRequestMapper> {
        RequestOutcomeRequestMapper()
    }

    single<AssetsLocalizationsResponseMapper> {
        AssetsLocalizationsResponseMapper()
    }

    single<EventsRequestMapper> {
        EventsRequestMapper()
    }

    single<PaymentsHistoryResponseMapper> {
        PaymentsHistoryResponseMapper()
    }

    single<CitizenEidAssociateRequestMapper> {
        CitizenEidAssociateRequestMapper()
    }

    single<CertificateAliasChangeRequestMapper> {
        CertificateAliasChangeRequestMapper()
    }

    single<MfaVerifyOtpCodeRequestMapper> {
        MfaVerifyOtpCodeRequestMapper()
    }

    single<MfaGenerateNewOtpCodeRequestMapper> {
        MfaGenerateNewOtpCodeRequestMapper()
    }

    single<ApplicationCompletionStatusResponseMapper> {
        ApplicationCompletionStatusResponseMapper()
    }

    single<SigningCheckUserStatusRequestMapper> {
        SigningCheckUserStatusRequestMapper()
    }

}