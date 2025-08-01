package com.digitall.eid.data.network.citizen.registration

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.citizen.registration.CitizenRegisterNewUserRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface CitizenRegistrationApi {

    @POST("mpozei/external/api/v1/citizens/register")
    suspend fun registerNewUser(
        @Body requestBody: CitizenRegisterNewUserRequest
    ): Response<EmptyResponse>
}