package com.digitall.eid.domain.usecase.nomenclatures

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class NomenclaturesGetReasonsUseCase: BaseUseCase {

    private val nomenclaturesNetworkRepository: NomenclaturesNetworkRepository by inject()

    companion object {
        private const val TAG = "NomenclaturesGetReasonsUseCaseTag"
    }

    fun invoke(): Flow<ResultEmittedData<List<NomenclaturesReasonsModel>>> {
        return nomenclaturesNetworkRepository.getReasons()
    }
}