package com.digitall.eid.data.network.citizen.forgotten

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.citizen.forgotten.password.CitizenForgottenPasswordRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface CitizenForgottenApi {

    @POST("mpozei/external/api/v1/citizens/forgotten-password")
    suspend fun forgottenPassword(
        @Body requestBody: CitizenForgottenPasswordRequest
    ): Response<EmptyResponse>

}