/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.create

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class CreateEmpowermentUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CreateEmpowermentUseCaseTag"
    }

    private val empowermentCreateNetworkRepository: EmpowermentCreateNetworkRepository by inject()

    fun invoke(data: EmpowermentCreateModel): Flow<ResultEmittedData<Unit>> {
        return empowermentCreateNetworkRepository.createEmpowerment(data)
    }

}