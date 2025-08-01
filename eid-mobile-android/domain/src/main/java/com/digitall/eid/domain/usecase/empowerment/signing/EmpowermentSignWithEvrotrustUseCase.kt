/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.empowerment.signing

import com.digitall.eid.domain.CHECK_STATUS_INTERVAL_DELAY
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.empowerment.signing.EmpowermentSigningSignRequestModel
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignDocumentRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustStatusEnum
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

class EmpowermentSignWithEvrotrustUseCase : BaseUseCase {

    companion object {
        private const val TAG = "EmpowermentSignWithEvrotrustUseCaseTag"
    }

    private val signingNetworkRepository: SigningNetworkRepository by inject()
    private val empowermentSigningNetworkRepository: EmpowermentSigningNetworkRepository by inject()

    @Volatile
    private var isSignPollingEnabled = true

    fun invoke(data: EmpowermentItem): Flow<ResultEmittedData<Unit>> = flow {
        signingNetworkRepository.checkEvrotrustUserStatus(
            data = SigningCheckUserStatusRequestModel(uid = data.uid)
        ).onEach { result ->
            result.onLoading {
                logDebug("checkEvrotrustUserStatus onSuccess", TAG)
                emit(ResultEmittedData.loading(model = null, message = "PLEASE_WAIT"))
            }.onSuccess { model, _, responseCode ->
                logDebug(
                    "checkEvrotrustUserStatus onSuccess readyToSign: ${model.readyToSign}",
                    TAG
                )
                if (model.readyToSign != true) {
                    logError("checkEvrotrustUserStatus onSuccess readyToSign != true", TAG)
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
                    signWithEvrotrust(
                        flow = this@flow,
                        empowermentId = data.id,
                        fileName = "${data.id}.xml",
                        uid = data.uid,
                        content = data.xmlRepresentation?.toBase64()!!,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("checkEvrotrustUserStatus onFailure", message, TAG)
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
        }.collect()
    }.flowOn(Dispatchers.IO)

    private suspend fun signWithEvrotrust(
        content: String,
        fileName: String,
        empowermentId: String,
        uid: String?,
        flow: FlowCollector<ResultEmittedData<Unit>>,
    ) {
        logDebug("signWithEvrotrust", TAG)
        signingNetworkRepository.signWithEvrotrust(
            request = SigningEvrotrustSignRequestModel(
                dateExpire = getCalendar(plusDays = 1).toServerDate(
                    dateFormat = ToServerDateFormats.WITH_MILLIS,
                ),
                documents = listOf(
                    SigningEvrotrustSignDocumentRequestModel(
                        content = content,
                        fileName = fileName,
                        contentType = "text/xml",
                    )
                ),
                uid = uid
            )
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("signWithEvrotrust onSuccess", TAG)
                val transactionID = model.transactions?.first()?.transactionID
                if (transactionID.isNullOrEmpty()) {
                    logError("signWithEvrotrust onSuccess but transactionID isNullOrEmpty", TAG)
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Transaction ID is empty",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                } else {
                    flow.emit(
                        ResultEmittedData.loading(
                            model = null,
                            message = "OPEN_EVROTRUST_APPLICATION"
                        )
                    )
                    checkStatus(
                        flow = flow,
                        empowermentId = empowermentId,
                        transactionId = transactionID,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("signWithEvrotrust onFailure", message, TAG)
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
                send(signingNetworkRepository.getEvrotrustStatus(transactionId = transactionId))
            }
        }.collect {
            it.collect { result ->
                result.onSuccess { model, _, responseCode ->
                    logDebug(
                        "checkStatus onSuccess status: ${model.status} processing: ${model.processing}",
                        TAG
                    )
                    when (model.status) {
                        SigningEvrotrustStatusEnum.PENDING.type -> {
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

                        SigningEvrotrustStatusEnum.REJECTED.type -> {
                            logError("checkStatus onSuccess status REJECTED", TAG)
                            isSignPollingEnabled = false
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    responseCode = responseCode,
                                    message = "Status is rejected",
                                    errorType = ErrorType.SERVER_DATA_ERROR,
                                )
                            )
                        }

                        SigningEvrotrustStatusEnum.SIGNED.type -> {
                            isSignPollingEnabled = false
                            downloadSignature(
                                flow = flow,
                                empowermentId = empowermentId,
                                transactionId = transactionId,
                            )
                        }

                        else -> {
                            logError("checkStatus onSuccess else", TAG)
                            isSignPollingEnabled = false
                            flow.emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = null,
                                    title = "Server error",
                                    responseCode = responseCode,
                                    errorType = ErrorType.SERVER_DATA_ERROR,
                                    message = "Number of attempts to check status exceeded",
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
        signingNetworkRepository.getEvrotrustDownload(
            transactionId = transactionId,
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("downloadSignature onSuccess", TAG)
                val content = model.first().content
                if (content.isNullOrEmpty()) {
                    logError(
                        "downloadSignature onSuccess content isNullOrEmpty",
                        TAG
                    )
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                            responseCode = responseCode,
                            message = "Signature content is empty",
                        )
                    )
                } else {
                    signEmpowerment(
                        flow = flow,
                        empowermentId = empowermentId,
                        detachedSignature = content,
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
                signatureProvider = "Evrotrust",
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