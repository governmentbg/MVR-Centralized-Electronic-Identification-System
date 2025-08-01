package com.digitall.eid.domain.repository.network.nomenclatures

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import kotlinx.coroutines.flow.Flow

interface NomenclaturesNetworkRepository {

    fun getReasons(): Flow<ResultEmittedData<List<NomenclaturesReasonsModel>>>
}