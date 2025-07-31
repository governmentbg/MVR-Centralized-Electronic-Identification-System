/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.authentication

import com.digitall.eid.data.models.network.authentication.request.AuthenticationCertificateRequest
import com.digitall.eid.data.models.network.authentication.request.AuthenticationChallengeRequest
import com.digitall.eid.data.models.network.authentication.request.BasicProfileAuthenticationRequest
import com.digitall.eid.data.models.network.authentication.response.AuthenticationChallengeResponse
import com.digitall.eid.data.models.network.authentication.response.AuthenticationResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthenticationApi {

    @POST("iscei/api/v1/auth/basic")
    suspend fun authenticateWithBasicProfile(
        @Body requestBody: BasicProfileAuthenticationRequest,
    ): Response<AuthenticationResponse>

    @POST("iscei/api/v1/auth/generate-authentication-challenge")
    suspend fun generateAuthenticationChallenge(
        @Body requestBody: AuthenticationChallengeRequest
    ): Response<AuthenticationChallengeResponse>

    @POST("iscei/api/v1/auth/mobile/certificate-login")
    suspend fun authenticateWithCertificate(
        @Body requestBody: AuthenticationCertificateRequest
    ): Response<AuthenticationResponse>

}