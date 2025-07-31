/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.cancel

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCancelNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class CancelEmpowermentFromMeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CreateEmpowermentUseCaseTag"
    }

    private val empowermentCancelNetworkRepository: EmpowermentCancelNetworkRepository by inject()

    fun invoke(
        reason: String,
        empowermentId: String,
    ): Flow<ResultEmittedData<Unit>> {
        return empowermentCancelNetworkRepository.cancelEmpowermentFromMe(
            reason = reason,
            empowermentId = empowermentId,
        )
    }


}