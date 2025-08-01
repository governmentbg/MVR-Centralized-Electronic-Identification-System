package com.digitall.eid.domain.repository.network.requests

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.requests.request.RequestOutcomeRequestModel
import com.digitall.eid.domain.models.requests.response.RequestResponseModel
import kotlinx.coroutines.flow.Flow

interface RequestsNetworkRepository {

    fun getAll(): Flow<ResultEmittedData<List<RequestResponseModel>>>

    fun setRequestOutcome(
        requestId: String?,
        outcome: RequestOutcomeRequestModel,
    ): Flow<ResultEmittedData<Unit>>

}