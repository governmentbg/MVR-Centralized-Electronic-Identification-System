/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.empowerment

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.signing.EmpowermentSigningSignRequestModel
import kotlinx.coroutines.flow.Flow

fun interface EmpowermentSigningNetworkRepository {

    fun signEmpowerment(
        empowermentId: String,
        request: EmpowermentSigningSignRequestModel,
    ): Flow<ResultEmittedData<Unit>>

}