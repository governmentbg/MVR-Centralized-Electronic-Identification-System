/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.applications

import com.digitall.eid.data.models.network.applications.create.ApplicationConfirmWithBaseProfileRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationConfirmWithEIDRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationEnrollWithBaseProfileRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationEnrollWithBaseProfileResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationEnrollWithEIDRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationEnrollWithEIDResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationGenerateUserDetailsXMLResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationSendSignatureRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationSendSignatureResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationSignWithBaseProfileRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationUpdateProfileRequest
import com.digitall.eid.data.models.network.applications.create.ApplicationUpdateProfileResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationUserDetailsResponse
import com.digitall.eid.data.models.network.applications.create.ApplicationDetailsXMLRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface ApplicationCreateApi {

    @GET("mpozei/external/api/v1/eidentities")
    suspend fun getUserDetails(): Response<ApplicationUserDetailsResponse>

    @POST("mpozei/external/api/v1/applications/generate-xml")
    suspend fun generateUserDetailsXML(
        @Body request: ApplicationDetailsXMLRequest,
    ): Response<ApplicationGenerateUserDetailsXMLResponse>

    @POST("mpozei/external/api/v1/applications")
    suspend fun sendSignature(
        @Body request: ApplicationSendSignatureRequest,
    ): Response<ApplicationSendSignatureResponse>

    @POST("mpozei/external/api/v1/applications/certificate-status-change/signed")
    suspend fun sendCertificateApplication(
        @Body request: ApplicationSendSignatureRequest,
    ): Response<ApplicationSendSignatureResponse>

    @POST("mpozei/external/api/v1/applications/sign")
    suspend fun signWithBaseProfile(
        @Body request: ApplicationSignWithBaseProfileRequest
    ): Response<String>

    @POST("mpozei/external/api/v1/mobile/certificates/enroll/base-profile")
    suspend fun enrollWithBaseProfile(
        @Body request: ApplicationEnrollWithBaseProfileRequest
    ): Response<ApplicationEnrollWithBaseProfileResponse>

    @POST("mpozei/external/api/v1/mobile/certificates/confirm")
    suspend fun confirmWithBaseProfile(
        @Body request: ApplicationConfirmWithBaseProfileRequest
    ): Response<String>

    @POST("mpozei/external/api/v1/mobile/certificates/enroll/eid")
    suspend fun enrollWithEID(
        @Body request: ApplicationEnrollWithEIDRequest
    ): Response<ApplicationEnrollWithEIDResponse>

    @POST("mpozei/external/api/v1/mobile/certificates/confirm/eid")
    suspend fun confirmWithEID(
        @Body request: ApplicationConfirmWithEIDRequest
    ): Response<String>

    @POST("mpozei/external/api/v1/mobile/verify-login")
    suspend fun updateProfile(
        @Body request: ApplicationUpdateProfileRequest
    ): Response<ApplicationUpdateProfileResponse>

}