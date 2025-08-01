package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationUpdateProfileRequestModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
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

class ApplicationSignWithoutProviderUseCase: BaseUseCase {

    companion object {
        private const val TAG = "ApplicationSignWithoutProviderUseCaseTag"
    }

    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

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
                    logDebug("generateXML xml: ${model.xml}",
                        TAG
                    )
                    updateProfile(
                        flow = flow,
                        firebaseId = firebaseId,
                        xml = model.xml.toBase64(),
                        mobileApplicationInstanceId = mobileApplicationInstanceId,
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("generateXML onFailure", message,
                    TAG
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

    private suspend fun updateProfile(
        xml: String,
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
                logDebug("updateProfile onSuccess",
                    TAG
                )
                sendXml(
                    xml = xml,
                    flow = flow,
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("updateProfile onFailure", message,
                    TAG
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

    private suspend fun sendXml(
        xml: String,
        flow: FlowCollector<ResultEmittedData<ApplicationSendSignatureResponseModel>>,
    ) {
        logDebug("sendSignature", TAG)
        applicationCreateNetworkRepository.sendSignature(
            data = ApplicationSendSignatureRequestModel(
                xml = xml,
                signature = null,
                signatureProvider = null,
            )
        ).onEach { result ->
            result.onSuccess { model, message, responseCode ->
                logDebug("sendSignature onSuccess",
                    TAG
                )
                flow.emit(
                    ResultEmittedData.success(
                        model = model,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("sendSignature onFailure", message,
                    TAG
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