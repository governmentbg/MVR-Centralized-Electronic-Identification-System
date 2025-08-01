/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.create

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceScopeModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetEmpowermentServiceScopeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetEmpowermentServiceScopeUseCaseTag"
    }

    private val empowermentCreateNetworkRepository: EmpowermentCreateNetworkRepository by inject()

    fun invoke(serviceId: String): Flow<ResultEmittedData<List<EmpowermentServiceScopeModel>>> {
        return empowermentCreateNetworkRepository.getEmpowermentServiceScope(
            serviceId = serviceId,
        )
    }

}