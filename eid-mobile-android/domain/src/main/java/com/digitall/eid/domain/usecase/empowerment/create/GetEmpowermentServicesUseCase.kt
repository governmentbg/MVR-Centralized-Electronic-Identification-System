/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.create

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.create.EmpowermentServiceModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.FlowCollector
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class GetEmpowermentServicesUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetEmpowermentProvidersUseCaseTag"
        private const val PAGE_SIZE = 50
    }

    private val empowermentCreateNetworkRepository: EmpowermentCreateNetworkRepository by inject()

    private var pageIndex = 1
    private var totalItems = 0

    private val data = mutableListOf<EmpowermentServiceModel>()

    fun invoke(
        provideId: String,
    ): Flow<ResultEmittedData<List<EmpowermentServiceModel>>> = flow {
        logDebug("invoke", TAG)
        pageIndex = 1
        data.clear()
        getNext(
            provideId = provideId,
            flow = this@flow,
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun getNext(
        provideId: String,
        flow: FlowCollector<ResultEmittedData<List<EmpowermentServiceModel>>>,
    ) {
        logDebug("getNext", TAG)
        empowermentCreateNetworkRepository.getEmpowermentServices(
            providerId = provideId,
            pageSize = PAGE_SIZE,
            pageIndex = pageIndex,
        ).onEach { result ->
            result.onLoading {
                logDebug("getNext onLoading", TAG)
                flow.emit(ResultEmittedData.loading(model = null))
            }.onSuccess { model, message, responseCode ->
                logDebug("getNext onSuccess", TAG)
                if (model.data != null) {
                    logDebug("getNext size: ${model.data.size}", TAG)
                    data.addAll(model.data)
                }
                if (model.pageIndex != null &&
                    model.totalItems != null &&
                    model.pageIndex * PAGE_SIZE <= model.totalItems
                ) {
                    totalItems = model.totalItems
                    pageIndex = model.pageIndex + 1
                    getNext(
                        provideId = provideId,
                        flow = flow,
                    )
                } else {
                    logDebug("getNext Ready", TAG)
                    flow.emit(
                        ResultEmittedData.success(
                            model = data,
                            message = message,
                            responseCode = responseCode,
                        )
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("getNext onFailure", message, TAG)
                flow.emit(
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
        }.collect()
    }

}