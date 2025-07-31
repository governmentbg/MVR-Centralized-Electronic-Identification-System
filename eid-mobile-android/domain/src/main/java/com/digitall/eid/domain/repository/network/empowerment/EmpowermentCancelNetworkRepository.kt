/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.empowerment

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.common.EmpowermentReasonModel
import kotlinx.coroutines.flow.Flow

interface EmpowermentCancelNetworkRepository {

    fun getEmpowermentFromMeCancelReasons(): Flow<ResultEmittedData<EmpowermentReasonModel>>

    fun getEmpowermentToMeCancelReasons(): Flow<ResultEmittedData<EmpowermentReasonModel>>

    fun cancelEmpowermentFromMe(
        reason: String,
        empowermentId: String,
    ): Flow<ResultEmittedData<Unit>>

    fun cancelEmpowermentToMe(
        reason: String,
        empowermentId: String,
    ): Flow<ResultEmittedData<Unit>>

}