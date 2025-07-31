package com.digitall.eid.domain.repository.network.administrators

import com.digitall.eid.domain.models.administrators.AdministratorFrontOfficeModel
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import kotlinx.coroutines.flow.Flow

interface AdministratorsNetworkRepository {

    fun getAdministrators(): Flow<ResultEmittedData<List<AdministratorModel>>>

    fun getAdministratorFrontOffices(eidAdministratorId: String): Flow<ResultEmittedData<List<AdministratorFrontOfficeModel>>>
}