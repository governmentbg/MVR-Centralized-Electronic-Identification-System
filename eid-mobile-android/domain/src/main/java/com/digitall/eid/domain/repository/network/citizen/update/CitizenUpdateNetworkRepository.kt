package com.digitall.eid.domain.repository.network.citizen.update

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.update.email.CitizenUpdateEmailRequestModel
import com.digitall.eid.domain.models.citizen.update.password.CitizenUpdatePasswordRequestModel
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import kotlinx.coroutines.flow.Flow

interface CitizenUpdateNetworkRepository {

    fun updateEmail(
        data: CitizenUpdateEmailRequestModel
    ): Flow<ResultEmittedData<Unit>>

    fun updatePassword(
        data: CitizenUpdatePasswordRequestModel
    ): Flow<ResultEmittedData<Unit>>

    fun updateInformation(
        data: CitizenUpdateInformationRequestModel
    ): Flow<ResultEmittedData<Unit>>

}