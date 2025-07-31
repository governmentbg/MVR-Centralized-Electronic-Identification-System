/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.cancel

import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCancelNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class GetEmpowermentFromMeCancelReasonsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetEmpowermentWithdrawReasonsUseCaseTag"
    }

    private val preferences: PreferencesRepository by inject()
    private val empowermentCancelNetworkRepository: EmpowermentCancelNetworkRepository by inject()

    fun invoke(): Flow<ResultEmittedData<List<String>>> = flow {
        val language = APPLICATION_LANGUAGE
        logDebug("language: ${language.type}", TAG)
        empowermentCancelNetworkRepository.getEmpowermentFromMeCancelReasons().onEach { result ->
            result.onLoading {
                logDebug("getEmpowermentWithdrawReasons onSuccess", TAG)
                emit(ResultEmittedData.loading(model = null))
            }.onSuccess { model, message, responseCode ->
                logDebug("getEmpowermentWithdrawReasons onSuccess", TAG)
                val filtered = model.translations!!.filter { translation ->
                    !translation.name.isNullOrEmpty() && translation.language == language.type
                }.map {
                    it.name!!
                }
                emit(
                    ResultEmittedData.success(
                        model = filtered,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("getEmpowermentWithdrawReasons onFailure", message, TAG)
                emit(
                    ResultEmittedData.error(
                        model = null,
                        title = title,
                        error = error,
                        message = message,
                        errorType = errorType,
                        responseCode = responseCode
                    )
                )
            }
        }.collect()
    }.flowOn(Dispatchers.IO)

}