/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.empowerment

import com.digitall.eid.data.mappers.network.empowerment.signing.EmpowermentSigningSignRequestMapper
import com.digitall.eid.data.network.empowerment.EmpowermentApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.signing.EmpowermentSigningSignRequestModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentSigningNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class EmpowermentSigningNetworkRepositoryImpl :
    EmpowermentSigningNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "EmpowermentSigningNetworkRepositoryTag"
    }

    private val empowermentApi: EmpowermentApi by inject()
    private val empowermentSigningSignRequestMapper: EmpowermentSigningSignRequestMapper by inject()

    override fun signEmpowerment(
        empowermentId: String,
        request: EmpowermentSigningSignRequestModel
    ) = flow {
        logDebug("signEmpowerment", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentApi.signEmpowerment(
                empowermentId = empowermentId,
                request = empowermentSigningSignRequestMapper.map(request),
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("signEmpowerment onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("signEmpowerment onFailure", message, TAG)
            emit(
                ResultEmittedData.error(
                    model = null,
                    error = error,
                    title = title,
                    message = message,
                    errorType = errorType,
                    responseCode = responseCode,
                )
            )
        }
    }.flowOn(Dispatchers.IO)

}