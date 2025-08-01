package com.digitall.eid.data.network.requests

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.requests.request.RequestOutcomeRequest
import com.digitall.eid.data.models.network.requests.response.RequestResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

interface RequestsApi {

    @GET("iscei/api/v1/approval-request/user")
    suspend fun getAll(): Response<List<RequestResponse>>

    @POST("iscei/api/v1/approval-request/outcome")
    suspend fun setRequestOutcome(
        @Query("approvalRequestId") approvalRequestId: String?,
        @Body requestBody: RequestOutcomeRequest
    ): Response<EmptyResponse>
    
}