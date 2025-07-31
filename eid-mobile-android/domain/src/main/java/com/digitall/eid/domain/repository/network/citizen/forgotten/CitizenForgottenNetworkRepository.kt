package com.digitall.eid.domain.repository.network.citizen.forgotten

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.forgotten.password.CitizenForgottenPasswordRequestModel
import kotlinx.coroutines.flow.Flow

interface CitizenForgottenNetworkRepository {

    fun forgottenPassword(
        data: CitizenForgottenPasswordRequestModel
    ): Flow<ResultEmittedData<Unit>>

}