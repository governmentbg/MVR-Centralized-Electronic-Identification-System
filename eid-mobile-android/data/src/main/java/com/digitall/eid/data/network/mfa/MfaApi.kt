package com.digitall.eid.data.network.mfa

import com.digitall.eid.data.models.network.authentication.response.AuthenticationResponse
import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.mfa.request.GenerateNewOtpCodeRequest
import com.digitall.eid.data.models.network.mfa.request.VerifyOtpCodeRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface MfaApi {

    @POST("iscei/api/v1/auth/verify-otp")
    suspend fun verifyOtpCode(
        @Body requestBody: VerifyOtpCodeRequest,
    ): Response<AuthenticationResponse>

    @POST("iscei/api/v1/auth/generate-otp")
    suspend fun generateNewOtpCode(
        @Body requestBody: GenerateNewOtpCodeRequest,
    ): Response<EmptyResponse>
}