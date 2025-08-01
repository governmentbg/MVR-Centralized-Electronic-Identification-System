package com.digitall.eid.data.network.events

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.events.request.EventRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface EventsApi {

    @POST("mpozei/external/api/v1/device/log")
    suspend fun logEvent(
        @Body request: EventRequest
    ): Response<EmptyResponse>
}