package com.digitall.eid.domain.models.events.request

data class EventRequestModel(
    val eventType: String?,
    val eventPayload: Map<String, Any>?,
)
