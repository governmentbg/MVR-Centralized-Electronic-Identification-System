package com.digitall.eid.domain.repository.network.citizen.eid.associate

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import kotlinx.coroutines.flow.Flow

interface CitizenEidAssociateNetworkRepository {

    fun associateEid(
        data: SignedChallengeRequestModel
    ): Flow<ResultEmittedData<Unit>>
}