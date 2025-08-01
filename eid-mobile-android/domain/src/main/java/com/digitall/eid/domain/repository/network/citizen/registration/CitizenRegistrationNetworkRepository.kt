package com.digitall.eid.domain.repository.network.citizen.registration

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.register.CitizenRegisterNewUserRequestModel
import kotlinx.coroutines.flow.Flow

interface CitizenRegistrationNetworkRepository {

    fun registerNewCitizen(
        data: CitizenRegisterNewUserRequestModel
    ): Flow<ResultEmittedData<Unit>>

}