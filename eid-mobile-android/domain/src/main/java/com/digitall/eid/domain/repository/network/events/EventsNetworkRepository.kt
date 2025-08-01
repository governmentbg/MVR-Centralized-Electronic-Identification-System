package com.digitall.eid.domain.repository.network.events

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.events.request.EventRequestModel
import kotlinx.coroutines.flow.Flow

interface EventsNetworkRepository {

    fun logEvent(data: EventRequestModel): Flow<ResultEmittedData<Unit>>

}