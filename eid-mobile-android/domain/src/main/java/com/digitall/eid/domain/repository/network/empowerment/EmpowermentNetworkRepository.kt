/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.empowerment

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentRequestModel
import com.digitall.eid.domain.models.empowerment.legal.EmpowermentLegalRequestModel
import kotlinx.coroutines.flow.Flow

interface EmpowermentNetworkRepository {

    fun getEmpowermentFromMe(
        data: EmpowermentRequestModel,
    ): Flow<ResultEmittedData<EmpowermentModel>>

    fun getEmpowermentToMe(
        data: EmpowermentRequestModel,
    ): Flow<ResultEmittedData<EmpowermentModel>>

    fun getEmpowermentLegal(
        data: EmpowermentLegalRequestModel,
    ): Flow<ResultEmittedData<EmpowermentModel>>

}