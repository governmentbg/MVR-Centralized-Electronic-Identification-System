package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.CHECK_STATUS_INTERVAL_DELAY
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.extensions.toTextDate
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignContentRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignSignaturePositionRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusCodeEnum
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusEnum
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
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

class CertificateApplicationSignWithBoricaUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CertificateApplicationSignWithBoricaUseCaseTag"
    }

    private val signingNetworkRepository: SigningNetworkRepository by inject()
    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    @Volatile
    private var isSignPollingEnabled = true

    fun invoke(
        data: ApplicationDetailsXMLRequestModel,
    ): Flow<ResultEmittedData<ApplicationSendSignatureResponseModel>> = flow {
        generateXML(
            data = data,
            flow = this@flow,
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun generateXML(
        data: ApplicationDetailsXMLRequestModel,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("generateXML", TAG)
        applicationCreateNetworkRepository.generateUserDetailsXML(
            data = data,
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("generateXML onSuccess", TAG)
                if (model.xml.isNullOrEmpty()) {
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "XML for signature is empty",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                } else {
                    checkUserStatus(
                        flow = flow,
                        xml = model.xml.toBase64(),
                        uid = data.citizenIdentifierNumber,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("generateXML onFailure", message, TAG)
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

    private suspend fun checkUserStatus(
        xml: String,
        uid: String?,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("checkUserStatus", TAG)
        signingNetworkRepository.checkBoricaUserStatus(
            data = SigningCheckUserStatusRequestModel(uid = uid)
        ).onEach { result ->
            result.onLoading {
                logDebug("checkUserStatus onLoading", TAG)
                flow.emit(ResultEmittedData.loading(model = null, message = "PLEASE_WAIT"))
            }.onSuccess { model, _, responseCode ->
                logDebug("checkUserStatus onSuccess responseCode: ${model.responseCode}", TAG)
                if (model.responseCode != "OK") {
                    logError("checkUserStatus onSuccess readyToSign != true", TAG)
                    flow.emit(
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
                        xml = xml,
                        uid = uid,
                        flow = flow,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("checkUserStatus onFailure", message, TAG)
                flow.emit(
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
    }

    private suspend fun signWithBorica(
        xml: String,
        uid: String?,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("signWithBorica", TAG)
        signingNetworkRepository.signWithBorica(
            request = SigningBoricaSignRequestModel(
                contents = listOf(
                    SigningBoricaSignContentRequestModel(
                        data = xml,
                        mediaType = "text/xml",
                        pagesVisualSignature = true,
                        confirmText = "Confirm sign",
                        contentFormat = "BINARY_BASE64",
                        fileName = "${
                            getCalendar().toTextDate(
                                dateFormat = UiDateFormats.WITH_TIME,
                            )
                        }.xml",
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
                    checkSigningStatus(
                        xml = xml,
                        flow = flow,
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
    private suspend fun checkSigningStatus(
        xml: String,
        transactionId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("checkSigningStatus", TAG)
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
                                    xml = xml,
                                    flow = flow,
                                    transactionId = model.signature,
                                )
                            }
                        }

                        else -> {
                            logError(
                                "checkStatus onSuccess else",
                                TAG
                            )
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
        xml: String,
        transactionId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
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
                    sendSignature(
                        xml = xml,
                        flow = flow,
                        signature = model.content.toBase64(),
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

    private suspend fun sendSignature(
        xml: String,
        signature: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("sendSignature", TAG)
        applicationCreateNetworkRepository.sendSignature(
            data = ApplicationSendSignatureRequestModel(
                xml = xml,
                signature = signature,
                signatureProvider = "BORICA",
            )
        ).onEach { result ->
            result.onSuccess { model, message, responseCode ->
                logDebug("sendSignature onSuccess", TAG)
                flow.emit(
                    ResultEmittedData.success(
                        model = model,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("sendSignature onFailure", message, TAG)
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