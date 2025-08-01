/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithEIDRequestModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
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

class ApplicationCreateConfirmWithEIDUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationConfirmWithEIDUseCaseTag"
        private const val SAVE_CERTIFICATE_STATUS_OK = "OK"
        private const val SAVE_CERTIFICATE_STATUS_ERROR = "ERROR"
    }

    private val cryptographyHelper: CryptographyHelper by inject()
    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    fun invoke(
        alias: String,
        certificate: String,
        applicationId: String,
        certificateChain: List<String>,
    ): Flow<ResultEmittedData<Unit>> = flow {
        logDebug("invoke", TAG)
        emit(ResultEmittedData.loading(model = null))
        confirmWithEID(
            alias = alias,
            flow = this@flow,
            certificate = certificate,
            applicationId = applicationId,
            certificateChain = certificateChain,
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun confirmWithEID(
        alias: String,
        certificate: String,
        applicationId: String,
        certificateChain: List<String>,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("confirmWithEID", TAG)
        val saveCertificateSuccess = cryptographyHelper.saveCertificateWithChainToKeyStore(
            alias = alias,
            certificate = certificate,
            certificateChain = certificateChain,
        )
        val status = if (saveCertificateSuccess) SAVE_CERTIFICATE_STATUS_OK
        else SAVE_CERTIFICATE_STATUS_ERROR
        applicationCreateNetworkRepository.confirmWithEID(
            data = ApplicationConfirmWithEIDRequestModel(
                reason = null,
                status = status,
                reasonText = null,
                applicationId = applicationId,
            )
        ).onEach { result ->
            result.onSuccess { model, message, responseCode ->
                logDebug("confirmWithEID onSuccess", TAG)
                if (!saveCertificateSuccess) {
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Certificate not saved",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                    return@onEach
                }
                if (model != "COMPLETED") {
                    cryptographyHelper.deleteCertificateWithChainFromKeyStore(
                        alias = alias,
                    )
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Not submitted",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                    return@onEach
                }
                flow.emit(
                    ResultEmittedData.success(
                        model = Unit,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("confirmWithEID onFailure", message, TAG)
                cryptographyHelper.deleteCertificateWithChainFromKeyStore(
                    alias = alias,
                )
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