/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.signing

import com.digitall.eid.domain.CHECK_STATUS_INTERVAL_DELAY
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.empowerment.signing.EmpowermentSigningSignRequestModel
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignContentRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignSignaturePositionRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusCodeEnum
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusEnum
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentSigningNetworkRepository
import com.digitall.eid.domain.repository.network.signing.SigningNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.DelicateCoroutinesApi
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.FlowCollector
import kotlinx.coroutines.flow.channelFlow
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class EmpowermentSignWithBoricaUseCase : BaseUseCase {

    companion object {
        private const val TAG = "EmpowermentSignWithBoricaUseCaseTag"
    }

    private val signingNetworkRepository: SigningNetworkRepository by inject()
    private val empowermentSigningNetworkRepository: EmpowermentSigningNetworkRepository by inject()

    @Volatile
    private var isSignPollingEnabled = true

    fun invoke(data: EmpowermentItem): Flow<ResultEmittedData<Unit>> = flow {
        signingNetworkRepository.checkBoricaUserStatus(
            data = SigningCheckUserStatusRequestModel(uid = data.uid)
        ).onEach { result ->
            result.onLoading {
                logDebug("checkBoricaUserStatus onLoading", TAG)
                emit(ResultEmittedData.loading(model = null, message = "PLEASE_WAIT"))
            }.onSuccess { model, _, responseCode ->
                logDebug("checkBoricaUserStatus onSuccess responseCode: ${model.responseCode}", TAG)
                if (model.responseCode != "OK") {
                    logError("checkBoricaUserStatus onSuccess readyToSign != true", TAG)
                    emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Not ready to sign",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                } else {
                    signWithBorica(
                        flow = this@flow,
                        empowermentId = data.id,
                        fileName = "${data.id}.xml",
                        data = data.xmlRepresentation?.toBase64()!!,
                        uid = data.uid,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("checkBoricaUserStatus onFailure", message, TAG)
                emit(
                    ResultEmittedData.error(
                        model = null,
                        error = error,
                        title = title,
                        message = message,
                        errorType = errorType,
                        responseCode = responseCode
                    )
                )
            }
        }.collect()
    }.flowOn(Dispatchers.IO)

    private suspend fun signWithBorica(
        fileName: String,
        empowermentId: String,
        data: String,
        uid: String?,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("signWithBorica", TAG)
        signingNetworkRepository.signWithBorica(
            request = SigningBoricaSignRequestModel(
                contents = listOf(
                   SigningBoricaSignContentRequestModel(
                       data = data,
                       fileName = fileName,
                       mediaType = "text/xml",
                       pagesVisualSignature = true,
                       confirmText = "Confirm sign",
                       contentFormat = "BINARY_BASE64",
                       signaturePosition = SigningBoricaSignSignaturePositionRequestModel(
                           pageNumber = 1,
                           imageXAxis = 20,
                           imageYAxis = 20,
                           imageHeight = 20,
                           imageWidth = 100,
                       )
                   )
                ),
                uid = uid,
            )
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("signWithBorica onSuccess callbackId: ${model.callbackId}", TAG)
                if (model.callbackId.isNullOrEmpty()) {
                    logError("signWithBorica onSuccess but callbackId isNullOrEmpty", TAG)
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Callback ID is empty",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                } else {
                    flow.emit(
                        ResultEmittedData.loading(model = null, message = "OPEN_BORICA_APPLICATION")
                    )
                    checkStatus(
                        flow = flow,
                        empowermentId = empowermentId,
                        transactionId = model.callbackId,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("signWithBorica onFailure", message, TAG)
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

    @OptIn(DelicateCoroutinesApi::class)
    private suspend fun checkStatus(
        empowermentId: String,
        transactionId: String,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("checkStatus", TAG)
        isSignPollingEnabled = true
        val checkStatusStartTime = System.currentTimeMillis()
        channelFlow {
            while (!isClosedForSend) {
                if (!isSignPollingEnabled) {
                    close()
                    return@channelFlow
                }

                delay(CHECK_STATUS_INTERVAL_DELAY)
                if (isSignPollingEnabled.not()) return@channelFlow
                send(signingNetworkRepository.getBoricaStatus(transactionId = transactionId))
            }
        }.collect {
            it.collect { result ->
                result.onSuccess { model, _, responseCode ->
                    logDebug(
                        "checkStatus onSuccess status: ${model.status} responseCode: ${model.responseCode}",
                        TAG
                    )
                    when {
                        model.status == SigningBoricaStatusCodeEnum.IN_PROGRESS.type &&
                                model.responseCode == SigningBoricaStatusCodeEnum.IN_PROGRESS.type -> {
                            flow.emit(ResultEmittedData.loading(null, model.message))
                            if (System.currentTimeMillis() - checkStatusStartTime >= SIGNING_REQUEST_TIMEOUT) {
                                isSignPollingEnabled = false
                                flow.emit(
                                    ResultEmittedData.error(
                                        model = null,
                                        error = null,
                                        title = "Server error",
                                        responseCode = responseCode,
                                        message = "Signing timeout reached",
                                        errorType = ErrorType.SERVER_DATA_ERROR,
                                    )
                                )
                            }
                        }

                        model.status == SigningBoricaStatusCodeEnum.REJECTED.type -> {
                            logError("checkStatus onSuccess REJECTED", TAG)
                            isSignPollingEnabled = false
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    responseCode = responseCode,
                                    message = "Status is rejected",
                                    errorType = ErrorType.ERROR_IN_LOGIC,
                                )
                            )
                        }

                        model.status == SigningBoricaStatusEnum.SIGNED.type &&
                                model.responseCode == SigningBoricaStatusCodeEnum.COMPLETED.type -> {
                            isSignPollingEnabled = false
                            if (model.signature.isNullOrEmpty()) {
                                logError(
                                    "checkStatus onSuccess SIGNED and COMPLETED but signature isNullOrEmpty",
                                    TAG
                                )
                                flow.emit(
                                    ResultEmittedData.error(
                                        model = null,
                                        error = null,
                                        title = "Server error",
                                        message = model.message,
                                        responseCode = responseCode,
                                        errorType = ErrorType.ERROR_IN_LOGIC,
                                    )
                                )
                            } else {
                                flow.emit(ResultEmittedData.loading(null, model.message))
                                downloadSignature(
                                    flow = flow,
                                    empowermentId = empowermentId,
                                    transactionId = model.signature,
                                )
                            }
                        }

                        else -> {
                            logError("checkStatus onSuccess else", TAG)
                            isSignPollingEnabled = false
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    message = model.message,
                                    responseCode = responseCode,
                                    errorType = ErrorType.ERROR_IN_LOGIC,
                                )
                            )
                        }
                    }
                }.onFailure { error, title, message, responseCode, errorType ->
                    logError("checkStatus onFailure", message, TAG)
                    isSignPollingEnabled = false
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
            }
        }
    }

    private suspend fun downloadSignature(
        empowermentId: String,
        transactionId: String,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("downloadSignature", TAG)
        signingNetworkRepository.getBoricaDownload(
            transactionId = transactionId,
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("downloadSignature onSuccess content: ${model.content}", TAG)
                if (model.content.isNullOrEmpty()) {
                    logError("downloadSignature onSuccess but content isNullOrEmpty", TAG)
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Signature content is empty",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                } else {
                    signEmpowerment(
                        flow = flow,
                        empowermentId = empowermentId,
                        detachedSignature = model.content,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("downloadSignature onFailure", message, TAG)
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

    private suspend fun signEmpowerment(
        empowermentId: String,
        detachedSignature: String,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("signEmpowerment", TAG)
        empowermentSigningNetworkRepository.signEmpowerment(
            empowermentId = empowermentId,
            request = EmpowermentSigningSignRequestModel(
                signatureProvider = "Borica",
                detachedSignature = detachedSignature,
            )
        ).onEach { result ->
            result.onSuccess { _, message, responseCode ->
                logDebug("signEmpowerment onSuccess", TAG)
                flow.emit(
                    ResultEmittedData.success(
                        model = Unit,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("signEmpowerment onFailure", message, TAG)
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