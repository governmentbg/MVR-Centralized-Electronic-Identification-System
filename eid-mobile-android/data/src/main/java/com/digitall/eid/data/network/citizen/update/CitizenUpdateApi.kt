package com.digitall.eid.data.network.citizen.update

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.base.ServerResponse
import com.digitall.eid.data.models.network.citizen.update.information.CitizenUpdateInformationRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Query

interface CitizenUpdateApi {

    @POST("mpozei/external/api/v1/citizens/update-email")
    suspend fun updateEmail(
        @Query("email") email: String?
    ): Response<EmptyResponse>

    @POST("mpozei/external/api/v1/citizens/update-password")
    suspend fun updatePassword(
        @Query("oldPassword") oldPassword: String?,
        @Query("newPassword") newPassword: String?,
        @Query("confirmPassword") confirmedPassword: String?
    ): Response<EmptyResponse>

    @PUT("mpozei/external/api/v1/citizens")
    suspend fun updateInformation(
        @Body requestBody: CitizenUpdateInformationRequest
    ): Response<ServerResponse>
}