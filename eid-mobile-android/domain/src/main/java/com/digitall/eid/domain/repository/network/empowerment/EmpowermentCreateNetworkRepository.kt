/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.empowerment

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentProvidersModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceScopeModel
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServicesModel
import kotlinx.coroutines.flow.Flow

interface EmpowermentCreateNetworkRepository {

    fun getEmpowermentProviders(
        pageSize: Int,
        pageIndex: Int,
    ): Flow<ResultEmittedData<EmpowermentProvidersModel>>

    fun createEmpowerment(
        data: EmpowermentCreateModel,
    ): Flow<ResultEmittedData<Unit>>

    fun getEmpowermentServiceScope(
        serviceId: String,
    ): Flow<ResultEmittedData<List<EmpowermentServiceScopeModel>>>

    fun getEmpowermentServices(
        pageSize: Int,
        pageIndex: Int,
        providerId: String,
    ): Flow<ResultEmittedData<EmpowermentServicesModel>>

}