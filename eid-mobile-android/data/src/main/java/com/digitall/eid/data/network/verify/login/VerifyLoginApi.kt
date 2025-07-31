package com.digitall.eid.data.network.verify.login

import com.digitall.eid.data.models.network.verify.login.request.VerifyLoginRequest
import com.digitall.eid.data.models.network.verify.login.response.VerifyLoginResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface VerifyLoginApi {

    @POST("mpozei/external/api/v1/mobile/verify-login")
    suspend fun verifyLogin(
        @Body requestBody: VerifyLoginRequest
    ): Response<VerifyLoginResponse>
}