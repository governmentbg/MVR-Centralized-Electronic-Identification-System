package com.digitall.eid.data.network.citizen.eid.associate

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST
import retrofit2.http.Query

interface CitizenEidAssociateApi {

    @POST("iscei/api/v1/auth/associate-profiles")
    suspend fun associateEid(
        @Query("client_id") clientId: String,
        @Body requestBody: SignedChallengeRequest
    ): Response<EmptyResponse>
}