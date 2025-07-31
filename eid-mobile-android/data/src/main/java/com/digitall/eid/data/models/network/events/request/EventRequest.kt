package com.digitall.eid.data.models.network.events.request

import com.google.gson.annotations.SerializedName

data class EventRequest(
    @SerializedName("eventType") val eventType: String?,
    @SerializedName("eventPayload") val eventPayload: Map<String, Any>?
)
