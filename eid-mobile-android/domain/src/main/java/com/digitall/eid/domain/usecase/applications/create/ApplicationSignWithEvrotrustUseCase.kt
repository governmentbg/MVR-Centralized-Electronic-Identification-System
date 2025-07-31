/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.CHECK_STATUS_INTERVAL_DELAY
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.extensions.toTextDate
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationUpdateProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignDocumentRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustStatusEnum
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

class ApplicationSignWithEvrotrustUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationSignWithEvrotrustUseCaseTag"
    }

    private val signingNetworkRepository: SigningNetworkRepository by inject()
    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    @Volatile
    private var isSignPollingEnabled = true

    fun invoke(
        firebaseId: String,
        mobileApplicationInstanceId: String,
        data: ApplicationDetailsXMLRequestModel,
    ): Flow<ResultEmittedData<ApplicationSendSignatureResponseModel>> = flow {
        logDebug(
            "invoke firebaseId: $firebaseId mobileApplicationInstanceId: $mobileApplicationInstanceId",
            TAG
        )
        generateXML(
            data = data,
            flow = this@flow,
            firebaseId = firebaseId,
            mobileApplicationInstanceId = mobileApplicationInstanceId,
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun generateXML(
        firebaseId: String,
        mobileApplicationInstanceId: String,
        data: ApplicationDetailsXMLRequestModel,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("generateXML", TAG)
        applicationCreateNetworkRepository.generateUserDetailsXML(
            data = data,
        ).onEach { result ->
            result.onLoading {
                flow.emit(ResultEmittedData.loading(model = null, message = "PLEASE_WAIT"))
            }.onSuccess { model, _, responseCode ->
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
                    logDebug("generateXML xml: ${model.xml}", TAG)
                    checkUserStatus(
                        flow = flow,
                        firebaseId = firebaseId,
                        xml = model.xml.toBase64(),
                        uid = data.citizenIdentifierNumber,
                        mobileApplicationInstanceId = mobileApplicationInstanceId,
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
        firebaseId: String,
        mobileApplicationInstanceId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("checkUserStatus", TAG)
        signingNetworkRepository.checkEvrotrustUserStatus(
            data = SigningCheckUserStatusRequestModel(uid = uid)
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug(
                    "checkUserStatus onSuccess readyToSign: ${model.readyToSign}",
                    TAG
                )
                if (model.readyToSign != true) {
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
                    signWithEvrotrust(
                        xml = xml,
                        uid = uid,
                        flow = flow,
                        firebaseId = firebaseId,
                        mobileApplicationInstanceId = mobileApplicationInstanceId,
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
                        responseCode = responseCode,
                    )
                )
            }
        }.collect()
    }

    private suspend fun signWithEvrotrust(
        xml: String,
        uid: String?,
        firebaseId: String,
        mobileApplicationInstanceId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("signWithEvrotrust", TAG)
        signingNetworkRepository.signWithEvrotrust(
            request = SigningEvrotrustSignRequestModel(
                dateExpire = getCalendar(plusDays = 1).toServerDate(
                    dateFormat = ToServerDateFormats.WITH_MILLIS,
                ),
                documents = listOf(
                    SigningEvrotrustSignDocumentRequestModel(
                        content = xml,
                        contentType = "text/xml",
                        fileName = "${
                            getCalendar().toTextDate(
                                dateFormat = UiDateFormats.WITH_TIME_SLASH,
                            )
                        }.xml",
                    )
                ),
                uid = uid,
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
                    checkSigningStatus(
                        xml = xml,
                        flow = flow,
                        firebaseId = firebaseId,
                        transactionId = transactionID,
                        mobileApplicationInstanceId = mobileApplicationInstanceId,
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
    private suspend fun checkSigningStatus(
        xml: String,
        firebaseId: String,
        transactionId: String,
        mobileApplicationInstanceId: String,
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
                                xml = xml,
                                flow = flow,
                                firebaseId = firebaseId,
                                transactionId = transactionId,
                                mobileApplicationInstanceId = mobileApplicationInstanceId,
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
        xml: String,
        firebaseId: String,
        transactionId: String,
        mobileApplicationInstanceId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("downloadSignature", TAG)
        signingNetworkRepository.getEvrotrustDownload(
            transactionId = transactionId,
        ).onEach { result ->
            result.onSuccess { model, _, responseCode ->
                logDebug("downloadSignature onSuccess", TAG)
                val content = model.first().content
                if (content.isNullOrEmpty()) {
                    logError("downloadSignature onSuccess content isNullOrEmpty", TAG)
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
                    updateProfile(
                        xml = xml,
                        flow = flow,
                        signature = content,
                        firebaseId = firebaseId,
                        mobileApplicationInstanceId = mobileApplicationInstanceId,
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

    private suspend fun updateProfile(
        xml: String,
        signature: String,
        firebaseId: String,
        mobileApplicationInstanceId: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("updateProfile", TAG)
        applicationCreateNetworkRepository.updateProfile(
            data = ApplicationUpdateProfileRequestModel(
                forceUpdate = true,
                firebaseId = firebaseId,
                mobileApplicationInstanceId = mobileApplicationInstanceId,
            )
        ).onEach { result ->
            result.onSuccess { _, _, _ ->
                logDebug("updateProfile onSuccess", TAG)
                sendSignature(
                    xml = xml,
                    flow = flow,
                    signature = signature,
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("updateProfile onFailure", message, TAG)
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
                signatureProvider = "Evrotrust",
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